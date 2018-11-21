using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Functions.EventsHub.Dto;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class EventsRouter
    {
        private static readonly StravaService StravaService = new StravaService();
        private static readonly IDictionary<string, bool> AthletesExistenceConfirmed = new ConcurrentDictionary<string, bool>();

        [FunctionName(FunctionsNames.EventsRouter)]
        public static async Task EventsHub([QueueTrigger(QueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.StravaEventsActivityAdd)] CloudQueue addActivityQueue,
            [Queue(QueueNames.StravaEventsActivityUpdate)] CloudQueue updateActivityQueue,
            [Queue(QueueNames.StravaEventsActivityDelete)] CloudQueue deleteActivityQueue,
            [Queue(QueueNames.StravaEventsAthleteDeauthorized)] CloudQueue deauthorizationQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.EventsRouter);

            var message = new ActivityData
            {
                AthleteId = @event.OwnerId.ToString(),
                ActivityId = @event.ObjectId.ToString()
            };

            var athleteExists =
                await AthleteExistsAndIsActiveAsync(message.AthleteId, configuration.ConnectionStrings.SqlDbConnectionString);
            if (!athleteExists)
            {
                // This can happen when the athlete is either deactivated (but authenticated) or has not yet been authorized.
                log.LogInformation($"Athlete with id: {message.AthleteId} does not exists.");
                return;
            }

            var json = JsonConvert.SerializeObject(message);

            if (@event.ObjectType == ObjectType.Activity)
            {
                switch (@event.AspectType)
                {
                    case AspectType.Create:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityAdd} queue.");
                        await addActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Update:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityUpdate} queue.");
                        await updateActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(1), null, null); // Handle quick add->update operation. An event informing about the addition of activity may appear later.
                        break;
                    case AspectType.Delete:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityDelete} queue.");
                        await deleteActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(1), null, null); // Handle quick add->delete operation. An event informing about the addition of activity may appear later.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                log.LogInformation(FunctionsNames.EventsRouter, "Event message has been processed.");
            }
            else if (@event.ObjectType == ObjectType.Athlete)
            {
                log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsAthleteDeauthorized} queue.");
                await deauthorizationQueue.AddMessageAsync(new CloudQueueMessage(json));
            }
            else
            {
                throw new Exception($"Unknown event type: {@event.ObjectType}");
            }
            log.LogFunctionEnd(FunctionsNames.EventsRouter);
        }

        private static async Task<bool> AthleteExistsAndIsActiveAsync(string athleteExternalId, string connectionString)
        {
            if (AthletesExistenceConfirmed.TryGetValue(athleteExternalId, out var exists))
            {
                return exists;
            }

            using (var conn = SqlConnectionFactory.Create(connectionString))
            {
                await conn.OpenWithRetryAsync();

                exists = await conn.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM dbo.Athletes WHERE ExternalId=@AthleteExternalId AND Active=1", new
                {
                    AthleteExternalId = athleteExternalId
                });
            }
            AthletesExistenceConfirmed.Add(athleteExternalId, exists);

            return exists;

        }

        [FunctionName(FunctionsNames.Events_DeauthorizedAthlete)]
        public static async Task Events_DeauthorizedAthlete([QueueTrigger(QueueNames.StravaEventsAthleteDeauthorized)] ActivityData @event,
            ILogger log,
            [Queue(AppQueueNames.NotificationsToSend, Connection = "AppQueuesStorage")] CloudQueue notificationsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_DeauthorizedAthlete);

            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=0 WHERE ExternalId=@AthleteId", new { @event.AthleteId });

                if (affectedRows == 1)
                {
                    log.LogInformation(FunctionsNames.Events_DeauthorizedAthlete, $"Successfully deauthorized athlete with id: {@event.AthleteId}.");

                    (string FirstName, string LastName) athlete = await conn.QuerySingleAsync<ValueTuple<string, string>>(
                        "SELECT FirstName, LastName from dbo.Athletes WHERE ExternalId=@AthleteId", new { @event.AthleteId });

                    var notification = new Notification
                    {
                        Recipients = new List<string> { configuration.Email.DefaultRecipient },
                        Subject = "Athlete revoked authorization",
                        HtmlContent = $@"
<p>Hi there,</p>
<p>Athlete: {athlete.FirstName} {athlete.LastName} [{@event.AthleteId}] revoked authorization.</p>"
                    };
                    await notificationsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(notification)));
                }
            }
            log.LogFunctionEnd(FunctionsNames.Events_DeauthorizedAthlete);
        }

        [FunctionName(FunctionsNames.Events_NewActivity)]
        public static async Task Events_NewActivity([QueueTrigger(QueueNames.StravaEventsActivityAdd)] ActivityData @event,
            ILogger log, 
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_NewActivity);

            var accessToken = await GetAccessToken(@event.AthleteId, configuration);

            StravaActivity activity;
            try
            {
                activity = StravaService.GetActivity(accessToken, @event.ActivityId);
            }
            catch (ActivityNotFoundException ex)
            {
                log.LogWarning(ex.Message);
                return;
            }

            var pendingActivity = new PendingRawActivity
            {
                Id = ActivityIdentity.Next(),
                ExternalId = activity.Id.ToString(),
                ExternalAthleteId = activity.Athlete.Id.ToString(),
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime),
                Source = "Strava"
            };

            var json = JsonConvert.SerializeObject(pendingActivity);
            var message = new CloudQueueMessage(json);
            await pendingRawActivitiesQueue.AddMessageAsync(message);
            log.LogFunctionEnd(FunctionsNames.Events_NewActivity);
        }

        [FunctionName(FunctionsNames.Events_UpdateActivity)]
        public static async Task Events_UpdateActivity([QueueTrigger(QueueNames.StravaEventsActivityUpdate)] ActivityData @event,
            ILogger log,
            [Queue(AppQueueNames.UpdateActivityRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_UpdateActivity);
            
            var accessToken = await GetAccessToken(@event.AthleteId, configuration);

            StravaActivity activity = null;
            try
            {
                StravaService.GetActivity(accessToken, @event.ActivityId);
            }
            catch (ActivityNotFoundException ex)
            {
                log.LogWarning(ex.Message);
                return;
            }

            var pendingActivity = new PendingRawActivity
            {
                ExternalId = activity.Id.ToString(),
                ExternalAthleteId = activity.Athlete.Id.ToString(),
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime),
                Source = "Strava"
            };

            var json = JsonConvert.SerializeObject(pendingActivity);
            var message = new CloudQueueMessage(json);
            await pendingRawActivitiesQueue.AddMessageAsync(message);
            log.LogFunctionEnd(FunctionsNames.Events_UpdateActivity);
        }

        private static async Task<string> GetAccessToken(string athleteId, ConfigurationRoot configuration)
        {
            string accessToken;
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                accessToken = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT AccessToken 
FROM dbo.[Strava.AccessTokens] AS Tokens
INNER JOIN dbo.Athletes AS Athletes ON (Athletes.Id = Tokens.AthleteId)
WHERE Athletes.ExternalId=@AthleteId AND Tokens.IsValid=1", new { AthleteId = athleteId });

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new AccessTokenNotFoundException(athleteId);
                }
            }

            accessToken = AccessTokensEncryptionService.Decrypt(accessToken,
                configuration.Strava.AccessTokensEncryptionKey);

            return accessToken;
        }

        [FunctionName(FunctionsNames.Events_DeleteActivity)]
        public static async Task Events_DeleteActivity([QueueTrigger(QueueNames.StravaEventsActivityDelete)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue deleteActivtiesQueue)
        {
            log.LogFunctionStart(FunctionsNames.Events_DeleteActivity);

            var json = JsonConvert.SerializeObject(new DeleteActivityRequest { ExternalId = @event.ActivityId });
            var message = new CloudQueueMessage(json);
            await deleteActivtiesQueue.AddMessageAsync(message);
            log.LogFunctionEnd(FunctionsNames.Events_DeleteActivity);
        }
    }
}
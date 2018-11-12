using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
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

        [FunctionName(FunctionsNames.EventsRouter)]
        public static async Task EventsHub([QueueTrigger(QueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.StravaEventsActivityAdd)] CloudQueue addActivityQueue,
            [Queue(QueueNames.StravaEventsActivityUpdate)] CloudQueue updateActivityQueue,
            [Queue(QueueNames.StravaEventsActivityDelete)] CloudQueue deleteActivityQueue,
            [Queue(QueueNames.StravaEventsAthleteDeauthorized)] CloudQueue deauthorizationQueue)
        {
            log.LogInformation($"{FunctionsNames.EventsRouter} function processed a request.");

            var message = new ActivityData
            {
                AthleteId = @event.OwnerId,
                ActivityId = @event.ObjectId
            };
            var json = JsonConvert.SerializeObject(message);

            if (@event.ObjectType == ObjectType.Activity)
            {
                switch (@event.AspectType)
                {
                    case AspectType.Create:
                        log.LogInformation($"{FunctionsNames.EventsRouter} adding message to {QueueNames.StravaEventsActivityAdd} queue.");
                        await addActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Update:
                        log.LogInformation($"{FunctionsNames.EventsRouter} adding message to {QueueNames.StravaEventsActivityUpdate} queue.");
                        await updateActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Delete:
                        log.LogInformation($"{FunctionsNames.EventsRouter} adding message to {QueueNames.StravaEventsActivityDelete} queue.");
                        await deleteActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                log.LogInformation($"{FunctionsNames.EventsRouter} messages has been added.");
            }
            else if (@event.ObjectType == ObjectType.Athlete)
            {
                log.LogInformation($"{FunctionsNames.EventsRouter} adding message to {QueueNames.StravaEventsAthleteDeauthorized} queue.");
                await deauthorizationQueue.AddMessageAsync(new CloudQueueMessage(json));
            }
            else
            {
                throw new Exception($"Unknown event type: {@event.ObjectType}");
            }
        }

        [FunctionName(FunctionsNames.Events_DeauthorizedAthlete)]
        public static async Task Events_DeauthorizedAthlete([QueueTrigger(QueueNames.StravaEventsAthleteDeauthorized)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.NotificationsToSend, Connection = "AppQueuesStorage")] CloudQueue notificationsQueue)
        {
            log.LogInformation($"{FunctionsNames.Events_DeauthorizedAthlete} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=0 WHERE ExternalId=@AthleteId", new { @event.AthleteId });

                if (affectedRows == 1)
                {
                    log.LogInformation($"{FunctionsNames.Events_DeauthorizedAthlete} successfully deauthorized athlete with id: {@event.AthleteId}.");

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
        }

        [FunctionName(FunctionsNames.Events_NewActivity)]
        public static async Task Events_NewActivity([QueueTrigger(QueueNames.StravaEventsActivityAdd)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.UpsertRawActivitiesRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.Events_NewActivity} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            var accessToken = await GetAccessToken(@event.AthleteId, configuration);

            var activity = StravaService.GetActivity(accessToken, @event.ActivityId);
            var pendingActivity = new PendingRawActivity
            {
                SourceActivityId = activity.Id,
                SourceAthleteId = activity.Athlete.Id,
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime),
                Source = "Strava",
                ActivityOperation = ActivityOperation.Create
            };

            var json = JsonConvert.SerializeObject(pendingActivity);
            var message = new CloudQueueMessage(json);
            await pendingRawActivitiesQueue.AddMessageAsync(message);
        }

        [FunctionName(FunctionsNames.Events_UpdateActivity)]
        public static async Task Events_UpdateActivity([QueueTrigger(QueueNames.StravaEventsActivityUpdate)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.UpsertRawActivitiesRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.Events_UpdateActivity} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            var accessToken = await GetAccessToken(@event.AthleteId, configuration);

            var activity = StravaService.GetActivity(accessToken, @event.ActivityId);
            var pendingActivity = new PendingRawActivity
            {
                SourceActivityId = activity.Id,
                SourceAthleteId = activity.Athlete.Id,
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime),
                Source = "Strava",
                ActivityOperation = ActivityOperation.Update
            };

            var json = JsonConvert.SerializeObject(pendingActivity);
            var message = new CloudQueueMessage(json);
            await pendingRawActivitiesQueue.AddMessageAsync(message);
        }

        private static async Task<string> GetAccessToken(int athleteId, ConfigurationRoot configuration)
        {
            string accessToken;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
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
            log.LogInformation($"{FunctionsNames.Events_DeleteActivity} function processed a request.");

            var message = new CloudQueueMessage(@event.ActivityId.ToString());
            await deleteActivtiesQueue.AddMessageAsync(message);
        }
    }
}
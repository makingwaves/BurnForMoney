using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Commands;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Shared.Repositories;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Functions.AddNewAthlete;
using BurnForMoney.Functions.Strava.Functions.EventsHub.Dto;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class EventsRouter
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.EventsRouter)]
        public static async Task EventsHub([QueueTrigger(QueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Table("Athletes", Connection = "AppStorage")] CloudTable athletesTable,
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

            var query = new TableQuery<AthleteEntity>
            {
                FilterString =
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, message.AthleteId)
            };
            var athleteEntity = (await athletesTable.ExecuteQuerySegmentedAsync(query, null)).FirstOrDefault();
            if (athleteEntity != null)
            {
                // This can happen when the athlete is either deactivated (but authenticated) or has not yet been authorized.
                log.LogInformation($"Athlete with id: {message.AthleteId} does not exists.");
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

                    (string Id, string FirstName, string LastName) athlete = await conn.QuerySingleAsync<ValueTuple<string, string, string>>(
                        "SELECT Id, FirstName, LastName from dbo.Athletes WHERE ExternalId=@AthleteId", new { @event.AthleteId });

                    var notification = new Notification
                    {
                        Recipients = new List<string> { configuration.Email.DefaultRecipient },
                        Subject = "Athlete revoked authorization",
                        HtmlContent = $@"
<p>Hi there,</p>
<p>Athlete: {athlete.FirstName} {athlete.LastName} [{@event.AthleteId}] revoked authorization.</p>"
                    };
                    await notificationsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(notification)));
                    await AccessTokensStore.DeactivateAccessTokenOfAsync(athlete.Id, configuration.Strava.AccessTokensKeyVaultUrl);
                }

            }
            log.LogFunctionEnd(FunctionsNames.Events_DeauthorizedAthlete);
        }

        [FunctionName(FunctionsNames.Events_NewActivity)]
        public static async Task Events_NewActivity([QueueTrigger(QueueNames.StravaEventsActivityAdd)] ActivityData @event,
            ILogger log,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue,
            [Table("Athletes", Connection = "AppStorage")] CloudTable athletesTable,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_NewActivity);

            var query = new TableQuery<AthleteEntity>
            {
                FilterString =
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, @event.AthleteId)
            };
            var queryResult = await athletesTable.ExecuteQuerySegmentedAsync(query, null);
            var athleteId = queryResult.FirstOrDefault()?.PartitionKey;

            var accessToken = await GetAccessTokenAsync(athleteId, configuration.Strava.AccessTokensKeyVaultUrl);

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
    
            var pendingActivity = new AddActivityCommand
            {
                Id = ActivityIdentity.Next(),
                ExternalId = activity.Id.ToString(),
                AthleteId = athleteId,
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
            [Table("Athletes", Connection = "AppStorage")] CloudTable athletesTable,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_UpdateActivity);

            var query = new TableQuery<AthleteEntity>
            {
                FilterString =
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, @event.AthleteId)
            };
            var queryResult = await athletesTable.ExecuteQuerySegmentedAsync(query, null);
            var athleteId = queryResult.FirstOrDefault()?.PartitionKey;

            var accessToken = await GetAccessTokenAsync(athleteId, configuration.Strava.AccessTokensKeyVaultUrl);

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

            var pendingActivity = new UpdateActivityCommand
            {
                Id = null,
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

        private static async Task<string> GetAccessTokenAsync(string athleteId, string keyVaultBaseUrl)
        {
            var secret = await AccessTokensStore.GetAccessTokenForAsync(athleteId, keyVaultBaseUrl);
            return secret.Value;
        }

        [FunctionName(FunctionsNames.Events_DeleteActivity)]
        public static async Task Events_DeleteActivity([QueueTrigger(QueueNames.StravaEventsActivityDelete)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue deleteActivtiesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_DeleteActivity);

            var activityRepository = new ActivityRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activity = await activityRepository.GetByExternalIdAsync(@event.ActivityId);

            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = activity.ActivityId, AthleteId = activity.AthleteId});
            var message = new CloudQueueMessage(json);
            await deleteActivtiesQueue.AddMessageAsync(message);
            log.LogFunctionEnd(FunctionsNames.Events_DeleteActivity);
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.EventsHub
{
    public static class EventsRouter
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Strava_EventsRouter)]
        public static async Task EventsHub([QueueTrigger(QueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.StravaEventsActivityAdd)] CloudQueue addActivityQueue,
            [Queue(QueueNames.StravaEventsActivityUpdate)] CloudQueue updateActivityQueue,
            [Queue(QueueNames.StravaEventsActivityDelete)] CloudQueue deleteActivityQueue)
        {
            log.LogInformation($"{FunctionsNames.Strava_EventsRouter} function processed a request.");

            if (@event.ObjectType == ObjectType.Activity)
            {
                var message = new ActivityData
                {
                    AthleteId = @event.OwnerId,
                    ActivityId = @event.ObjectId
                };
                var json = JsonConvert.SerializeObject(message);

                switch (@event.AspectType)
                {
                    case AspectType.Create:
                        log.LogInformation($"{FunctionsNames.Strava_EventsRouter} adding message to {QueueNames.StravaEventsActivityAdd} queue.");
                        await addActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Update:
                        log.LogInformation($"{FunctionsNames.Strava_EventsRouter} adding message to {QueueNames.StravaEventsActivityUpdate} queue.");
                        await updateActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Delete:
                        log.LogInformation($"{FunctionsNames.Strava_EventsRouter} adding message to {QueueNames.StravaEventsActivityDelete} queue.");
                        await deleteActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                log.LogInformation($"{FunctionsNames.Strava_EventsRouter} messages has been added.");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [FunctionName(FunctionsNames.Strava_Events_NewActivity)]
        public static async Task Strava_Events_NewActivity([QueueTrigger(QueueNames.StravaEventsActivityAdd)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingRawActivities)] CloudQueue pendingRawActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.Strava_Events_NewActivity} function processed a request.");

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

        [FunctionName(FunctionsNames.Strava_Events_UpdateActivity)]
        public static async Task Strava_Events_UpdateActivity([QueueTrigger(QueueNames.StravaEventsActivityUpdate)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingRawActivities)] CloudQueue pendingRawActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.Strava_Events_UpdateActivity} function processed a request.");

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
                accessToken = await conn.QuerySingleAsync<string>(@"SELECT AccessToken 
FROM dbo.[Strava.AccessTokens] AS Tokens
INNER JOIN dbo.Athletes AS Athletes ON (Athletes.Id = Tokens.AthleteId)
WHERE Athletes.ExternalId=@AthleteId", new { AthleteId = athleteId });

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new Exception($"Cannot find an access token fot athlete: {athleteId}");
                }
            }

            accessToken = AccessTokensEncryptionService.Decrypt(accessToken,
                configuration.Strava.AccessTokensEncryptionKey);

            return accessToken;
        }

        [FunctionName(FunctionsNames.Strava_Events_DeleteActivity)]
        public static async Task Strava_Events_DeleteActivity([QueueTrigger(QueueNames.StravaEventsActivityDelete)] ActivityData @event,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_Events_NewActivity} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(@"DELETE FROM dbo.Activities WHERE ActivityId=@ActivityId", new { @event.ActivityId });
                if (affectedRows == 0)
                {
                    throw new Exception($"Failed to remove activity with id: {@event.ActivityId}");
                }
            }
        }
    }
}
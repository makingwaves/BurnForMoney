using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions
{
    public static class CollectAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.CollectAthleteActivities)]
        public static async Task Run([QueueTrigger(QueueNames.CollectAthleteActivities)]int athleteId,
            [Queue(AppQueueNames.PendingRawActivities, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue, 
            [Queue(QueueNames.UnauthorizedAccessTokens)] CloudQueue unauthorizedAccessTokensQueue,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.CollectAthleteActivities} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            string encryptedAccessToken;
            string accessToken;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                encryptedAccessToken = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT AccessToken
FROM dbo.[Strava.AccessTokens]
WHERE AthleteId = @AthleteId AND IsValid=1", new { AthleteId = athleteId });

                if (string.IsNullOrWhiteSpace(encryptedAccessToken))
                {
                    throw new Exception($"Cannot find valid access token for athlete: {athleteId}.");
                }

                accessToken = AccessTokensEncryptionService.Decrypt(encryptedAccessToken,
                    configuration.Strava.AccessTokensEncryptionKey);
            }

            try
            {
                var activities = StravaService.GetActivities(accessToken, GetFirstDayOfTheMonth(DateTime.UtcNow));
                foreach (var stravaActivity in activities)
                {
                    var pendingActivity = new PendingRawActivity
                    {
                        SourceActivityId = stravaActivity.Id,
                        SourceAthleteId = stravaActivity.Athlete.Id,
                        ActivityType = stravaActivity.Type.ToString(),
                        StartDate = stravaActivity.StartDate,
                        DistanceInMeters = stravaActivity.Distance,
                        MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(stravaActivity.MovingTime),
                        Source = "Strava"
                    };

                    var json = JsonConvert.SerializeObject(pendingActivity);
                    var message = new CloudQueueMessage(json);
                    await pendingRawActivitiesQueue.AddMessageAsync(message);
                }
            }
            catch (UnauthorizedRequestException ex)
            {
                log.LogError(ex, ex.Message);
                await unauthorizedAccessTokensQueue.AddMessageAsync(new CloudQueueMessage(encryptedAccessToken));
            }
        }

        private static DateTime GetFirstDayOfTheMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }
    }
}

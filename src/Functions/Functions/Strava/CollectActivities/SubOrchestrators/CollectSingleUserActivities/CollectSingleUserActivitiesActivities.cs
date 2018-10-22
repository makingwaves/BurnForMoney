using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators.CollectSingleUserActivities
{
    public static class CollectSingleUserActivitiesActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Strava_A_CollectSingleUserActivities)]
        public static async Task A_CollectSingleUserActivitiesAsync([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingRawActivities)] CloudQueue pendingRawActivitiesQueue, [Queue(QueueNames.UnauthorizedAccessTokens)] CloudQueue unauthorizedAccessTokensQueue)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_CollectSingleUserActivities} function processed a request.");

            var (athleteId, encryptedAccessToken, fromDate) = context.GetInput<ValueTuple<int, string, DateTime>>();
            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            var accessToken = await AccessTokensEncryptionService.DecryptAsync(encryptedAccessToken,
                configuration.ConnectionStrings.KeyVaultConnectionString);
            log.LogInformation("Access token has been decrypted.");
            try
            {
                var activities = StravaService.GetActivities(accessToken, fromDate);
                foreach (var stravaActivity in activities)
                {
                    var pendingActivity = new PendingRawActivity
                    {
                        ActivityId = stravaActivity.Id,
                        AthleteId = athleteId,
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
            catch (UnauthorizedTokenException ex)
            {
                log.LogError(ex, ex.Message);
                await unauthorizedAccessTokensQueue.AddMessageAsync(new CloudQueueMessage(encryptedAccessToken));
            }

        }

        [FunctionName(FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete)]
        public static async Task A_UpdateLastUpdateDateOfTheUpdatedAthleteAsync([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingActivities)] CloudQueue pendingActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete} function processed a request.");

            (string athleteId, DateTime lastUpdate) = context.GetInput<(string, DateTime)>();

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(@"
IF NOT EXISTS (SELECT * FROM dbo.[Athletes.UpdateHistory] WITH (UPDLOCK) WHERE AthleteId = @AthleteId)
    INSERT dbo.[Athletes.UpdateHistory] (AthleteId, LastUpdate)
    VALUES (@AthleteId, @LastUpdate);
ELSE
UPDATE dbo.[Athletes.UpdateHistory] SET LastUpdate=@LastUpdate WHERE AthleteId=@AthleteId", new
                {
                    AthleteId = athleteId,
                    LastUpdate = lastUpdate
                });
                if (affectedRows != 1)
                {
                    throw new Exception("Failed to update LastUpdate date.");
                }
            }
        }
    }
}
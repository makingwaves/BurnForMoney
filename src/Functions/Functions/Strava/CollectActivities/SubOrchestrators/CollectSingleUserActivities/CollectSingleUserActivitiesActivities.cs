using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators.CollectSingleUserActivities
{
    public static class CollectSingleUserActivitiesActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Strava_A_DecryptAccessToken)]
        public static async Task<string> A_DecryptAccessTokenAsync([ActivityTrigger]string encryptedAccessToken, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_DecryptAccessToken} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = secret.Value;

            var decryptedToken = Cryptography.DecryptString(encryptedAccessToken, accessTokenEncryptionKey);
            log.LogInformation("Access token has been decrypted.");
            return decryptedToken;
        }

        [FunctionName(FunctionsNames.Strava_A_CollectSingleUserActivities)]
        public static async Task A_CollectSingleUserActivitiesAsync([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingRawActivities)] CloudQueue pendingRawActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_CollectSingleUserActivities} function processed a request.");

            var (athleteId, accessToken, fromDate) = context.GetInput<ValueTuple<int, string, DateTime>>();

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
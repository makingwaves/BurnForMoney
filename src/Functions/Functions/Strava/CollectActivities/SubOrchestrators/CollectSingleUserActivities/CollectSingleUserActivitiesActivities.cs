using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Helpers;
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
        [FunctionName(FunctionsNames.A_DecryptAccessToken)]
        public static async Task<string> A_DecryptAccessTokenAsync([ActivityTrigger]string encryptedAccessToken, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_DecryptAccessToken} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = secret.Value;

            var decryptedToken = Cryptography.DecryptString(encryptedAccessToken, accessTokenEncryptionKey);
            log.LogInformation("Access token has been decrypted.");
            return decryptedToken;
        }

        [FunctionName(FunctionsNames.A_RetrieveSingleUserActivities)]
        public static async Task A_RetrieveSingleUserActivitiesAsync([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.PendingActivities)] CloudQueue pendingActivitiesQueue)
        {
            log.LogInformation($"{FunctionsNames.A_RetrieveSingleUserActivities} function processed a request.");

            var (accessToken, from) = context.GetInput<ValueTuple<string, DateTime>>();

            var stravaService = new StravaService();
            var activities = stravaService.GetActivities(accessToken, from);

            foreach (var stravaActivity in activities)
            {
                var json = JsonConvert.SerializeObject(stravaActivity);
                var message = new CloudQueueMessage(json);
                await pendingActivitiesQueue.AddMessageAsync(message);
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
                await conn.ExecuteAsync("UPDATE dbo.[Strava.Athletes] SET LastUpdate=@LastUpdate WHERE AthleteId=@AthleteId AND Active=@Active", new
                {
                    AthleteId = athleteId,
                    LastUpdate = lastUpdate,
                    Active = true
                });
            }

        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.O_CollectStravaActivities)]
        public static async Task O_CollectStravaActivitiesAsync(ILogger log,
            [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            var optimize = context.GetInput<bool?>() ?? true;

            if (!context.IsReplaying)
            {
                log.LogInformation(
                    $"Orchestration function `{FunctionsNames.O_CollectStravaActivities}` received a request. {{optimize}}: {optimize}.");
            }

            // 1. Get active athletes
            var athletes =
                await context.CallActivityAsync<Athlete[]>(FunctionsNames.A_GetAthletesWithAccessTokens,
                    ActivityInput.Empty);
            if (athletes.Length == 0)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] cannot find any active athletes.");
                return;
            }

            if (!context.IsReplaying)
            {
                log.LogInformation(
                    $"[{FunctionsNames.O_CollectStravaActivities}] Retrieved data of {athletes.Length} active athletes.");
            }

            // 2. Collect activities of all athletes
            var tasks = new Task[athletes.Length];
            for (int i = 0; i < athletes.Length; i++)
            {
                tasks[i] = context.CallSubOrchestratorAsync("", (athletes[i], optimize));
            }


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! CATCH EXCEPTIONS AND IGNORE THEM

            await Task.WhenAll(tasks);
        }
    }

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
    }

}

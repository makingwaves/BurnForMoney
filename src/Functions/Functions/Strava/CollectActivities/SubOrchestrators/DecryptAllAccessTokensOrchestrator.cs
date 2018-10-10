using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators
{
    public static class DecryptAllAccessTokensOrchestrator
    {
        [FunctionName(FunctionsNames.O_DecryptAllAccessTokens)]
        public static async Task<string[]> O_DecryptAllAccessTokens(ILogger log,
            [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            var encryptedAccessTokens = context.GetInput<string[]>();

            var decryptionTasks = new Task<string>[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                decryptionTasks[i] = context.CallActivityAsync<string>(
                    FunctionsNames.A_DecryptAccessToken, encryptedAccessTokens[i]);
            }
            var decryptedAccessTokens = await Task.WhenAll(decryptionTasks);
            return decryptedAccessTokens;
        }
    }

    public static class DecryptAllAccessTokensActivities
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
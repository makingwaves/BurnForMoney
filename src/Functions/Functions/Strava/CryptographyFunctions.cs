using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class CryptographyFunctions
    {
        private static string _accessTokenEncryptionKey;

        [FunctionName(FunctionsNames.A_EncryptAccessToken)]
        public static async Task<string> A_EncryptAccessToken([ActivityTrigger]string accessToken, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_EncryptAccessToken} function processed a request.");
            await LoadSettingsAsync(context);

            var encryptedToken = Cryptography.EncryptString(accessToken, _accessTokenEncryptionKey);
            log.LogInformation("Access token has been encrypted.");
            return encryptedToken;
        }

        [FunctionName(FunctionsNames.A_DecryptAccessToken)]
        public static async Task<string> A_DecryptAccessToken([ActivityTrigger]string encryptedAccessToken, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_DecryptAccessToken} function processed a request.");
            await LoadSettingsAsync(context);

            var decryptedToken = Cryptography.DecryptString(encryptedAccessToken, _accessTokenEncryptionKey);
            log.LogInformation("Access token has been decrypted.");
            return decryptedToken;
        }

        private static async Task LoadSettingsAsync(ExecutionContext context)
        {
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            if (string.IsNullOrEmpty(_accessTokenEncryptionKey))
            {
                var keyVaultClient = KeyVaultClientFactory.Create();
                var secret = await keyVaultClient.GetSecretAsync(configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                    .ConfigureAwait(false);
                _accessTokenEncryptionKey = secret.Value;
            }
        }
    }
}
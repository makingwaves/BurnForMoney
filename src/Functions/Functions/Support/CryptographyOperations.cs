using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Support
{
    public static class CryptographyOperations
    {
        [FunctionName(FunctionsNames.Support_EncryptString)]
        public static async Task<IActionResult> RunEncryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/encryptstring")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_EncryptString} function processed a request.");

            string text = req.Query["text"];
            if (string.IsNullOrWhiteSpace(text))
            {
                log.LogWarning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            var configuration = ApplicationConfiguration.GetSettings(context);
            var keyVaultConnectionString = configuration.ConnectionStrings.KeyVaultConnectionString;
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedText = Cryptography.EncryptString(text, encryptionKey);

            return new OkObjectResult(encryptedText);
        }

        [FunctionName(FunctionsNames.Support_DecryptString)]
        public static async Task<IActionResult> RunDecryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/decryptstring")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_DecryptString} function processed a request.");

            string text = req.Query["text"];
            if (string.IsNullOrWhiteSpace(text))
            {
                log.LogWarning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            text = FillMissingSpecialCharacters(text);

            var configuration = ApplicationConfiguration.GetSettings(context);
            var keyVaultConnectionString = configuration.ConnectionStrings.KeyVaultConnectionString;
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedText = Cryptography.DecryptString(text, encryptionKey);

            return new OkObjectResult(encryptedText);
        }

        private static string FillMissingSpecialCharacters(string text) => text.Replace(" ", "+");

        private static async Task<string> GetEncryptionKeyAsync(string keyVaultConnectionString)
        {
            var keyVaultClient = KeyVaultClientFactory.Create();

            var secret = await keyVaultClient.GetSecretAsync(keyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                .ConfigureAwait(false);
            return secret.Value;
        }
    }
}

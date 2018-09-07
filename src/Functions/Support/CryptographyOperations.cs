using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Support
{
    public static class CryptographyOperations
    {
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();

        [FunctionName("EncryptString")]
        public static async Task<IActionResult> RunEncryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/encryptstring")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("EncryptString function processed a request.");

            string text = req.Query["text"];
            if (string.IsNullOrWhiteSpace(text))
            {
                log.Warning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            var keyVaultConnectionString = Configuration.GetSettings(context).ConnectionStrings.KeyVaultConnectionString;
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedText = Cryptography.EncryptString(text, encryptionKey);

            return new OkObjectResult(encryptedText);
        }

        [FunctionName("DecryptString")]
        public static async Task<IActionResult> RunDecryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/decryptstring")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("DecryptString function processed a request.");

            string text = req.Query["text"];
            if (string.IsNullOrWhiteSpace(text))
            {
                log.Warning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            text = FillMissingSpecialCharacters(text);

            var keyVaultConnectionString = Configuration.GetSettings(context).ConnectionStrings.KeyVaultConnectionString;
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedText = Cryptography.DecryptString(text, encryptionKey);

            return new OkObjectResult(encryptedText);
        }

        private static string FillMissingSpecialCharacters(string text) => text.Replace(" ", "+");

        private static async Task<string> GetEncryptionKeyAsync(string keyVaultConnectionString)
        {
            var keyVaultClient = KeyVaultClientFactory.Create();

            var secret = await keyVaultClient.GetSecretAsync(keyVaultConnectionString + "secrets/stravaAccessTokensEncryptionKey")
                .ConfigureAwait(false);
            return secret.Value;
        }
    }
}

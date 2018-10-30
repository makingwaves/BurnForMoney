using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Functions;
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
        public static IActionResult RunEncryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/encryptstring/{text}")]HttpRequest req, ILogger log, ExecutionContext context, string text)
        {
            log.LogInformation($"{FunctionsNames.Support_EncryptString} function processed a request.");

            if (string.IsNullOrWhiteSpace(text))
            {
                log.LogWarning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            var configuration = ApplicationConfiguration.GetSettings(context);
            var encryptedText = Cryptography.EncryptString(text, configuration.Strava.AccessTokensEncryptionKey);

            return new OkObjectResult(encryptedText);
        }

        [FunctionName(FunctionsNames.Support_DecryptString)]
        public static IActionResult RunDecryptString([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/decryptstring/{text}")]HttpRequest req, ILogger log, ExecutionContext context, string text)
        {
            log.LogInformation($"{FunctionsNames.Support_DecryptString} function processed a request.");

            if (string.IsNullOrWhiteSpace(text))
            {
                log.LogWarning("Function invoked with incorrect parameters. [text] is null or empty.");
                return new BadRequestObjectResult("Text is required.");
            }

            text = FillMissingSpecialCharacters(text);

            var configuration = ApplicationConfiguration.GetSettings(context);
            var encryptedText = Cryptography.DecryptString(text, configuration.Strava.AccessTokensEncryptionKey);

            return new OkObjectResult(encryptedText);
        }

        private static string FillMissingSpecialCharacters(string text) => text.Replace(" ", "+");
    }
}

using System.Threading.Tasks;
using BurnForMoney.Functions.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class CryptographyOperationsFunc
    {
        [FunctionName(FunctionsNames.Support_EncryptString)]
        public static async Task<IActionResult> EncryptStringAsync([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/encryptstring")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_EncryptString} function processed a request.");

            var data =  await req.ReadAsStringAsync();
            var postData = JsonConvert.DeserializeObject<CryptographyPostData>(data);

            if (string.IsNullOrWhiteSpace(postData.EncryptionKey))
            {
                return new BadRequestObjectResult($"{nameof(postData.EncryptionKey)} is required.");
            }
            if (string.IsNullOrWhiteSpace(postData.Text))
            {
                return new BadRequestObjectResult($"{nameof(postData.Text)} is required.");
            }

            var encryptedText = Cryptography.EncryptString(postData.Text, postData.EncryptionKey);

            return new OkObjectResult(encryptedText);
        }

        [FunctionName(FunctionsNames.Support_DecryptString)]
        public static async Task<IActionResult> DecryptStringAsync([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/decryptstring")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_DecryptString} function processed a request.");

            var data = await req.ReadAsStringAsync();
            var postData = JsonConvert.DeserializeObject<CryptographyPostData>(data);

            if (string.IsNullOrWhiteSpace(postData.EncryptionKey))
            {
                return new BadRequestObjectResult($"{nameof(postData.EncryptionKey)} is required.");
            }
            if (string.IsNullOrWhiteSpace(postData.Text))
            {
                return new BadRequestObjectResult($"{nameof(postData.Text)} is required.");
            }

            var encryptedText = Cryptography.DecryptString(postData.Text, postData.EncryptionKey);

            return new OkObjectResult(encryptedText);
        }
    }

    internal class CryptographyPostData
    {
        public string EncryptionKey { get; set; }
        public string Text { get; set; }
    }
}

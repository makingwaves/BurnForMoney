using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions
{
    public static class AuthorizeStravaUser
    {
        [FunctionName("AuthorizeStravaUser")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, TraceWriter log, [Queue(QueueNames.AuthorizationCodes)] CloudQueue authorizationCodesQueue)
        {
            log.Info("AuthorizeStravaUser function processed a request.");

            string code = req.Query["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                log.Info("Function invoked with incorrect parameters. [code] is null or empty.");
                return new BadRequestObjectResult("Code is required.");
            }

            await InsertCodeToAuthorizationQueueAsync(code, authorizationCodesQueue).ConfigureAwait(false);
            log.Info($"Inserted authorization code to {QueueNames.AuthorizationCodes} queue.");

            return new OkObjectResult("Ok");
        }

        private static async Task InsertCodeToAuthorizationQueueAsync(string code, CloudQueue queue)
        {
            var message = new CloudQueueMessage(code);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
        }
    }
}

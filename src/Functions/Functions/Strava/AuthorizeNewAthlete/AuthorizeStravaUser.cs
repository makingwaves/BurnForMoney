using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeStravaUser
    {
        private const int AuthorisationCodeLength = 40;
        private const string StravaAuthorizationUrl = "https://www.strava.com/oauth/authorize";
        private const string AzureHostUrl = "https://functions.azure.com";

        [FunctionName(FunctionsNames.AuthenticateStravaUser)]
        public static async Task<IActionResult> RunAuthorizeStravaUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/authenticate")]
            HttpRequest req, ILogger log, [Queue(QueueNames.AuthorizationCodes)] CloudQueue authorizationCodesQueue,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.AuthenticateStravaUser} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(context);

            var referer = req.Headers["Referer"].FirstOrDefault() ?? "null";
            log.LogInformation($"Request referer: [{referer}].");
            if (!configuration.IsLocalEnvironment && !IsRequestRefererValid(referer))
            {
                log.LogWarning($"Request referer [{referer}] is not authorized.");
                return new UnauthorizedResult();
            }

            string error = req.Query["error"];
            if (!string.IsNullOrWhiteSpace(error))
            {
                log.LogWarning($"Error occured during athlete authorization. Error code: [{error}].");
                return new BadRequestObjectResult($"An error occured, Error code: [{error}]");
            }

            string code = req.Query["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                log.LogWarning("Function invoked with incorrect parameters. [code] is null or empty.");
                return new BadRequestObjectResult("Code is required.");
            }

            if (code.Length != AuthorisationCodeLength)
            {
                log.LogWarning($"The provided code is invalid. Authorization code should be {AuthorisationCodeLength} chars long, but was {code.Length}.");
                return new BadRequestObjectResult("The provided code is invalid.");
            }

            await InsertCodeToAuthorizationQueueAsync(code, authorizationCodesQueue, log).ConfigureAwait(false);

            return new OkObjectResult("Authorization completed.");
        }
        
        private static async Task InsertCodeToAuthorizationQueueAsync(string code, CloudQueue queue, ILogger log)
        {
            var message = new CloudQueueMessage(code);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
            log.LogInformation($"Inserted authorization code to {QueueNames.AuthorizationCodes} queue.");
        }

        private static bool IsRequestRefererValid(string referer) => !string.IsNullOrEmpty(referer) && (referer.StartsWith(StravaAuthorizationUrl) || referer.StartsWith(AzureHostUrl));
    }
}
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
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
        private static ILogger _log;
        private const string StravaAuthorizationUrl = "https://www.strava.com/oauth/authorize";
        private const string AzureHostUrl = "https://functions.azure.com";

        // Authorize application to get athlete's data
        [FunctionName(FunctionsNames.AuthorizeStravaUser)]
        public static async Task<IActionResult> RunAuthorizeStravaUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/authorize")]
            HttpRequest req, ILogger log, [Queue(QueueNames.AuthorizationCodes)] CloudQueue authorizationCodesQueue,
            ExecutionContext context)
        {
            _log = log;
            _log.LogInformation($"{FunctionsNames.AuthorizeStravaUser} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(context);

            var referer = req.Headers["Referer"].FirstOrDefault();
            if (!configuration.IsLocalEnvironment && !IsRequestRefererValid(referer))
            {
                log.LogWarning($"Request referer [{referer ?? "null"}] is not authorized.");
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
                log.LogWarning($"The provided code is invalid. Authorisation code should be {AuthorisationCodeLength} chars long, but was {code.Length}.");
                return new BadRequestObjectResult("The provided code is invalid.");
            }

            await InsertCodeToAuthorizationQueueAsync(code, authorizationCodesQueue).ConfigureAwait(false);

            return new OkObjectResult("Authorization completed.");
        }
        
        private static async Task InsertCodeToAuthorizationQueueAsync(string code, CloudQueue queue)
        {
            var message = new CloudQueueMessage(code);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
            _log.LogInformation($"Inserted authorization code to {QueueNames.AuthorizationCodes} queue.");
        }

        private static bool IsRequestRefererValid(string referer) => !string.IsNullOrEmpty(referer) && (referer.StartsWith(StravaAuthorizationUrl) || referer.StartsWith(AzureHostUrl));
    }
}
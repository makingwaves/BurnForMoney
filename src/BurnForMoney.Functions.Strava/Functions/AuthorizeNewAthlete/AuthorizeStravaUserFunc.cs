using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeStravaUserFunc
    {
        private const int AuthorisationCodeLength = 40;
        private const string StravaAuthorizationUrl = "https://www.strava.com/oauth/authorize";
        private const string AzureHostUrl = "https://functions.azure.com";

        [FunctionName(FunctionsNames.AuthenticateUser)]
        public static async Task<IActionResult> RunAuthorizeStravaUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/authenticate")]
            HttpRequest req, ILogger log, [Queue(QueueNames.AuthorizationCodes)] CloudQueue authorizationCodesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.AuthenticateUser);

            var referer = req.Headers["Referer"].FirstOrDefault() ?? "null";
            log.LogInformation(FunctionsNames.AuthenticateUser, $"Request referer: [{referer}].");
            if (!configuration.IsLocalEnvironment && !IsRequestRefererValid(referer))
            {
                log.LogWarning(FunctionsNames.AuthenticateUser, $"Request referer [{referer}] is not authorized.");
                return new UnauthorizedResult();
            }

            string error = req.Query["error"];
            if (!string.IsNullOrWhiteSpace(error))
            {
                log.LogWarning(FunctionsNames.AuthenticateUser, $"Error occured during athlete authorization. Error code: [{error}].");
                return new BadRequestObjectResult($"An error occured, Error code: [{error}]");
            }

            string code = req.Query["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                log.LogWarning(FunctionsNames.AuthenticateUser, "Function invoked with incorrect parameters. [code] is null or empty.");
                return new BadRequestObjectResult("Code is required.");
            }

            if (code.Length != AuthorisationCodeLength)
            {
                log.LogWarning(FunctionsNames.AuthenticateUser, $"The provided code is invalid. Authorization code should be {AuthorisationCodeLength} chars long, but was {code.Length}.");
                return new BadRequestObjectResult("The provided code is invalid.");
            }

            await InsertCodeToAuthorizationQueueAsync(code, authorizationCodesQueue, log).ConfigureAwait(false);

            log.LogFunctionEnd(FunctionsNames.AuthenticateUser);
            return new RedirectResult(configuration.Strava.ConfirmationPageUrl);
        }
        
        private static async Task InsertCodeToAuthorizationQueueAsync(string code, CloudQueue queue, ILogger log)
        {
            var message = new CloudQueueMessage(code);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
            log.LogInformation(FunctionsNames.AuthenticateUser, $"Inserted authorization code to {QueueNames.AuthorizationCodes} queue.");
        }

        private static bool IsRequestRefererValid(string referer) => !string.IsNullOrEmpty(referer) && (referer.StartsWith(StravaAuthorizationUrl) || referer.StartsWith(AzureHostUrl));
    }
}
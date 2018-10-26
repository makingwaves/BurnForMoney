using System;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Shared.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class CreateWebhooksSubscription
    {
        private static readonly StravaService StravaService = new StravaService();
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        [FunctionName(FunctionsNames.Strava_CreateWebookSubscription)]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "strava/subscription/create")] HttpRequest req, 
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_CreateWebookSubscription} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            var callbackUrl = $"{configuration.HostName}/strava/events/push";
            var functionKey = GetCallbackFunctionKey(configuration.HostName, FunctionsNames.Strava_PushEvents, log);

            var subscriptionValidation = StravaService.CreateSubscription(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                callbackUrl, functionKey.Value);
            Cache.Set("challenge", subscriptionValidation.Challenge, TimeSpan.FromSeconds(5));
            log.LogInformation($"{FunctionsNames.Strava_CreateWebookSubscription} Temporarily cached 'challenge' token: {subscriptionValidation.Hub.Challenge}");

            return new OkResult();
        }

        private static FunctionKey GetCallbackFunctionKey(string host, string functionName, ILogger log)
        {
            var rest = new RestClient(host);
            var restRequest = new RestRequest($"/admin/functions/{functionName}/keys/default");
            log.LogInformation($"{FunctionsNames.Strava_CreateWebookSubscription} Sending request for an auth key of the {functionName} function.");
            var result = rest.Execute(restRequest);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                log.LogInformation($"{FunctionsNames.Strava_CreateWebookSubscription} Received auth key.");
                return JsonConvert.DeserializeObject<FunctionKey>(result.Content);
            }

            throw new Exception($"Failed to get auth key for callback function. Error code: `{result.StatusCode}`, Message: `{result.ErrorMessage}`");
        }

        [FunctionName(FunctionsNames.Strava_WebhooksCallbackValidation)]
        public static IActionResult ValidateCallback([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/events/push")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_WebhooksCallbackValidation} function processed a request.");

            var challenge = Cache.Get("challenge");
            if (challenge == null)
            {
                return new BadRequestResult();
            }
            log.LogInformation($"{FunctionsNames.Strava_WebhooksCallbackValidation} Received 'challenge' token: {challenge}.");

            return new OkObjectResult($"{{\"hub.challenge\":\"{challenge}\"}}");
        }

        [FunctionName(FunctionsNames.Strava_PushEvents)]
        public static async Task PushEvents([HttpTrigger(AuthorizationLevel.Function, "post", Route = "strava/events/push")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_PushEvents} function processed a request.");


        }
    }

    public class FunctionKey
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
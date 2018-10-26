using System;
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

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class CreateWebhooksSubscription
    {
        private static readonly StravaWebhooksService StravaWebhooksService = new StravaWebhooksService();
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        [FunctionName(FunctionsNames.Strava_CreateWebhookSubscription)]
        public static async Task<IActionResult> Strava_CreateWebhookSubscription([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "strava/subscription/create")] HttpRequest req, 
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_CreateWebhookSubscription} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            var data = await req.ReadAsStringAsync();
            log.LogInformation($"{FunctionsNames.Strava_CreateWebhookSubscription} function executed with the following data: <{data}>.");
            
            var hostname = configuration.HostName;
            if (!string.IsNullOrWhiteSpace(data))
            {
                var postData = JsonConvert.DeserializeObject<CreateSubscriptionPostData>(data);
                hostname = postData.Hostname;
            }

            var callbackUrl = $"{hostname}/api/strava/events/hub";
            var callbackToken = Guid.NewGuid().ToString("N");
            Cache.Set("callbackToken", callbackToken, TimeSpan.FromSeconds(10));
            log.LogInformation($"{FunctionsNames.Strava_CreateWebhookSubscription} Generated callback token: {callbackToken}.");

            log.LogInformation($"{FunctionsNames.Strava_CreateWebhookSubscription} Creating subscription for a client: <{configuration.Strava.ClientId}> and callback url: {callbackUrl}.");
            StravaWebhooksService.CreateSubscription(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                callbackUrl, callbackToken);

            return new OkResult();
        }

        private class CreateSubscriptionPostData
        {
            public string Hostname { get; set; }
        }

        [FunctionName(FunctionsNames.Strava_WebhooksCallbackValidation)]
        public static IActionResult ValidateCallback([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/events/hub")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_WebhooksCallbackValidation} function processed a request.");

            string mode = req.Query["hub.mode"];
            string verifyToken = req.Query["hub.verify_token"];
            string challenge = req.Query["hub.challenge"];

            var callbackToken = Cache.Get("callbackToken");

            if (mode != "subscribe" || !verifyToken.Equals(callbackToken))
            {
                return new BadRequestResult();
            }

            log.LogInformation($"{FunctionsNames.Strava_WebhooksCallbackValidation} Rquest validated.");
            return new OkObjectResult($"{{\"hub.challenge\":\"{challenge}\"}}");
        }

        [FunctionName(FunctionsNames.Strava_EventsHub)]
        public static async Task PushEvents([HttpTrigger(AuthorizationLevel.Function, "post", Route = "strava/events/hub")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_EventsHub} function processed a request.");

        }

        [FunctionName(FunctionsNames.Strava_ViewWebhookSubscription)]
        public static async Task<IActionResult> Strava_ViewWebhookSubscription([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "strava/subscription/view")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_ViewWebhookSubscription} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            try
            {
                var subscription = StravaWebhooksService.ViewSubscription(configuration.Strava.ClientId,
                    configuration.Strava.ClientSecret);
                return new OkObjectResult(subscription);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
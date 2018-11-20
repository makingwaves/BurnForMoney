using System;
using System.Net.Http;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class WebhooksSubscriptionFunc
    {
        private static readonly StravaWebhooksService StravaWebhooksService = new StravaWebhooksService();
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string CallbackToken = "013a818de91f490695f8f642c9b511c3";

        [FunctionName(FunctionsNames.CreateWebhookSubscription)]
        public static async Task<IActionResult> CreateWebhookSubscription([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "strava/subscription/create")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.CreateWebhookSubscription);

            var data = await req.ReadAsStringAsync();
            log.LogInformation(FunctionsNames.CreateWebhookSubscription, $"Function executed with the following data: <{data}>.");

            var hostname = configuration.HostName;
            if (!string.IsNullOrWhiteSpace(data))
            {
                var postData = JsonConvert.DeserializeObject<CreateSubscriptionPostData>(data);
                hostname = postData.Hostname;
            }

            var callbackUrl = $"{hostname}/api/strava/events/hub";
            await WarmUpValidationFunction(callbackUrl, log);
            
            log.LogInformation(FunctionsNames.CreateWebhookSubscription, $"Creating subscription for a client: <{configuration.Strava.ClientId}> and callback url: {callbackUrl}.");
            StravaWebhooksService.CreateSubscription(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                callbackUrl, CallbackToken);

            log.LogFunctionEnd(FunctionsNames.CreateWebhookSubscription);
            return new OkResult();
        }

        private static async Task<HttpResponseMessage> WarmUpValidationFunction(string url, ILogger log)
        {
            var response = await HttpClient.GetAsync(url + "?warmUp=true");
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Successfully warmed up callback function: '{url}'");
            }
            else
            {
                log.LogError($"Failed to warm up callback function: '{url}'. Response: {(int)response.StatusCode + " : " + response.ReasonPhrase}");
            }

            return response;
        }

        private class CreateSubscriptionPostData
        {
            public string Hostname { get; set; }
        }

        [FunctionName(FunctionsNames.WebhooksCallbackValidation)]
        public static IActionResult ValidateCallback([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "strava/events/hub")] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart(FunctionsNames.WebhooksCallbackValidation);

            if (bool.TryParse(req.Query["warmUp"], out var _))
            {
                log.LogInformation(FunctionsNames.WebhooksCallbackValidation, "Processing warm up execution.");
                return new OkResult();
            }

            string verifyToken = req.Query["hub.verify_token"];
            string challenge = req.Query["hub.challenge"];

            if (string.IsNullOrWhiteSpace(verifyToken) || string.IsNullOrWhiteSpace(challenge))
            {
                return new BadRequestResult();
            }
            if (!verifyToken.Equals(CallbackToken))
            {
                log.LogError(FunctionsNames.WebhooksCallbackValidation, $"Received callback token: {verifyToken} is not valid with token: {CallbackToken}.");
                return new BadRequestResult();
            }

            log.LogInformation(FunctionsNames.WebhooksCallbackValidation, "Request validated.");
            var json = JsonConvert.SerializeObject(new ChallengeObject { Challenge = challenge });
            log.LogFunctionEnd(FunctionsNames.WebhooksCallbackValidation);
            return new OkObjectResult(json);
        }

        private class ChallengeObject
        {
            [JsonProperty("hub.challenge")]
            public string Challenge { get; set; }
        }

        [FunctionName(FunctionsNames.ViewWebhookSubscription)]
        public static IActionResult ViewWebhookSubscription([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "strava/subscription")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.ViewWebhookSubscription);

            try
            {
                var subscription = StravaWebhooksService.ViewSubscription(configuration.Strava.ClientId,
                    configuration.Strava.ClientSecret);
                log.LogFunctionEnd(FunctionsNames.ViewWebhookSubscription);
                return new OkObjectResult(subscription);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName(FunctionsNames.DeleteWebhookSubscription)]
        public static IActionResult DeleteWebhookSubscription([HttpTrigger(AuthorizationLevel.Admin, "delete", Route = "strava/subscription/{id}")] HttpRequest req,
            ILogger log, int id, [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.ViewWebhookSubscription);

            if (id <= 0)
            {
                return new BadRequestObjectResult("Subscription id required.");
            }
            
            try
            {
                var subscription = StravaWebhooksService.DeleteSubscription(configuration.Strava.ClientId,
                    configuration.Strava.ClientSecret, id);
                log.LogFunctionEnd(FunctionsNames.ViewWebhookSubscription);
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
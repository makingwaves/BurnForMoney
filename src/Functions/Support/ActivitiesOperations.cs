using System.Net.Http;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Support
{
    public static class ActivitiesOperations
    {
        [FunctionName(FunctionsNames.Support_Strava_CollectActivities)]
        public static async Task<HttpResponseMessage> CollectActivitiesAsync([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/strava/collectactivities")]HttpRequestMessage req, ILogger log, 
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_CollectActivities} function processed a request.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, null);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
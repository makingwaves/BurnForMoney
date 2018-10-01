using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Support
{
    public static class ActivitiesOperations
    {
        [FunctionName(FunctionsNames.Support_Strava_Activities_Collect)]
        public static async Task<HttpResponseMessage> Support_Strava_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/strava/activities/collect")]HttpRequestMessage req, ILogger log, 
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Activities_Collect} function processed a request.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, null);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName(FunctionsNames.Support_Strava_Activities_CollectMonthlyStatistics)]
        public static async Task<HttpResponseMessage> Support_Strava_Activities_MohlyStatisticsCollect([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/strava/activities/collectmonthlystatistics")]HttpRequest req, ILogger log,
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Activities_CollectMonthlyStatistics} function processed a request.");

            string month = req.Query["month"];
            if (string.IsNullOrWhiteSpace(month))
            {
                log.LogWarning("Function invoked with incorrect parameters. [month] is null or empty.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            string year = req.Query["year"];
            if (string.IsNullOrWhiteSpace(year))
            {
                log.LogWarning("Function invoked with incorrect parameters. [year] is null or empty.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CalculateMonthlyAthleteResults, new DateTime(int.Parse(year), int.Parse(month), 1));

            return starter.CreateCheckStatusResponse(new HttpRequestMessage(), instanceId);
        }
    }
}
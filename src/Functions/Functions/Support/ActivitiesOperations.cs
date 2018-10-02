using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Support
{
    public static class ActivitiesOperations
    {
        [FunctionName(FunctionsNames.Support_Strava_Activities_Collect)]
        public static async Task<IActionResult> Support_Strava_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/strava/activities/collect")]HttpRequest req, ILogger log, 
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Activities_Collect} function processed a request.");
            string optimize = req.Query["optimize"];
            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, optimize);

            var payload = starter.CreateHttpManagementPayload(instanceId);
            return new OkObjectResult(payload);
        }

        [FunctionName(FunctionsNames.Support_Strava_Activities_CollectMonthlyStatistics)]
        public static async Task<IActionResult> Support_Strava_Activities_MonthlyStatisticsCollect([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/strava/activities/collectmonthlystatistics")]HttpRequest req, ILogger log,
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Activities_CollectMonthlyStatistics} function processed a request.");

            string month = req.Query["month"];
            if (string.IsNullOrWhiteSpace(month))
            {
                var errorMessage = "Function invoked with incorrect parameters. [month] is null or empty.";
                log.LogWarning(errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }

            string year = req.Query["year"];
            if (string.IsNullOrWhiteSpace(year))
            {
                var errorMessage = "Function invoked with incorrect parameters. [year] is null or empty.";
                log.LogWarning(errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CalculateMonthlyAthleteResults, new DateTime(int.Parse(year), int.Parse(month), 1));

            var payload = starter.CreateHttpManagementPayload(instanceId);
            return new OkObjectResult(payload);
        }
    }
}
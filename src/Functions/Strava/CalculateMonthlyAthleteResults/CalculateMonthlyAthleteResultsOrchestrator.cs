using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsOrchestrator
    {
        [FunctionName(FunctionsNames.O_CalculateMonthlyAthleteResults)]
        public static async Task Run([OrchestrationTrigger] DurableOrchestrationContext context, ILogger log, ExecutionContext executionContext)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_CalculateMonthlyAthleteResults}` received a request.");
            }

            // 1. Get last month activities
            var lastMonthActivities = await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_GetLastMonthActivities, null);
            if (lastMonthActivities.Count == 0)
            {
                log.LogWarning($"[{FunctionsNames.O_CollectStravaActivities}] cannot find any activities from last month.");
                return;
            }
        }
    }
}
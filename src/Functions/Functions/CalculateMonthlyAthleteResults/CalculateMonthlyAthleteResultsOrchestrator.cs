using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults
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

            var (month, year) = context.GetInput<(int, int)>();

            // 1. Get activities
            var activities = await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_GetActivitiesFromGivenMonth, (month, year));
            if (activities.Count == 0)
            {
                log.LogWarning($"[{FunctionsNames.A_GetActivitiesFromGivenMonth}] cannot find any activities from given date: {month}/{year}.");
                return;
            }

            // 3. Submit athlete monthly results
            await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_SubmitAthleteMonthlyResults, (activities, (month, year)));
        }
    }
}

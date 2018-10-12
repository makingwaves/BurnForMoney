using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CalculateMonthlyAthleteResults
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

            // 2. Group activities by athlete
            var aggregatedActivities = await context.CallActivityAsync<List<AthleteMonthlyResult>>(FunctionsNames.A_GroupActivitiesByAthlete, activities);
            
            // 3. Store aggregated athlete monthly results
            await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_StoreAggregatedAthleteMonthlyResults, (aggregatedActivities, (month, year)));
        }
    }
}
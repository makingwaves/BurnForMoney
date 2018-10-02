using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
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

            var date = context.GetInput<DateTime>();

            // 1. Get activities
            var activities = await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_GetActivitiesFromGivenMonth, date);
            if (activities.Count == 0)
            {
                log.LogWarning($"[{FunctionsNames.A_GetActivitiesFromGivenMonth}] cannot find any activities from given date: {date.Month}/{date.Year}.");
                return;
            }

            // 2. Store aggregated athlete results
            await context.CallActivityAsync<List<Activity>>(FunctionsNames.A_StoreAggregatedAthleteResults, new CalculateMonthlyAthleteResultsActivities.A_StoreAggregatedAthleteResults_Input
            {
                Date = date,
                Activities = activities
            });
        }
    }
}
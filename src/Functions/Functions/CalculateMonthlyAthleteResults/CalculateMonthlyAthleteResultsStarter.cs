using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsStarter
    {
        // every hour
        [FunctionName(FunctionsNames.CalculateMonthlyAthleteResults)]
        public static async Task CalculateMonthlyAthleteResults([TimerTrigger("0 0 * * * *")]TimerInfo timer, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.CalculateMonthlyAthleteResults} timer trigger processed a request at {DateTime.UtcNow}.");

            var date = DateTime.UtcNow;
            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CalculateMonthlyAthleteResults, (date.Month, date.Year));
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CalculateMonthlyAthleteResults}` with ID = `{instanceId}`.");
        }

        // first day of the month at 1:00
        [FunctionName(FunctionsNames.CalculateMonthlyAthleteResultsFromPreviousMonth)]
        public static async Task CalculateMonthlyAthleteResultsFromPreviousMonth([TimerTrigger("0 0 1 1 * *")]TimerInfo timer, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.CalculateMonthlyAthleteResultsFromPreviousMonth} timer trigger processed a request at {DateTime.UtcNow}.");

            var date = DateTime.UtcNow.AddMonths(-1);
            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CalculateMonthlyAthleteResults, (date.Month, date.Year));
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CalculateMonthlyAthleteResults}` with ID = `{instanceId}`.");
        }
    }
}
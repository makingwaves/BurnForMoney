using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsStarter
    {
        // first day of the month at 5:00
        [FunctionName(FunctionsNames.CalculateMonthlyAthleteResultsOnFirstDayOfTheMonth)]
        public static async Task Run([TimerTrigger("0 0 5 1 * * *")]TimerInfo timer, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.CalculateMonthlyAthleteResultsOnFirstDayOfTheMonth} timer trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CalculateMonthlyAthleteResults, null);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CalculateMonthlyAthleteResults}` with ID = `{instanceId}`.");
        }
    }

    public class Activity
    {
        public int AthleteId { get; set; }
        public long ActivityId { get; set; }
        public DateTime ActivityTime { get; set; }
        public string ActivityType { get; set; }
        public double Distance { get; set; }
        public double MovingTime { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }
}
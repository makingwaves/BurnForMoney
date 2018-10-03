using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesStarter
    {
        // at *:55 every day
        [FunctionName(FunctionsNames.CollectStravaActivitiesInEveryHour)]
        public static async Task RunTimerStarter([TimerTrigger("0 55 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.CollectStravaActivitiesInEveryHour} timer trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, true);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CollectStravaActivities}` with ID = `{instanceId}`.");
        }
    }
}
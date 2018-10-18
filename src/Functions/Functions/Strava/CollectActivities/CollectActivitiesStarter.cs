using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesStarter
    {
        // at *:55 every day
        [FunctionName(FunctionsNames.Strava_CollectStravaActivitiesInEveryHour)]
        public static async Task RunTimerStarter([TimerTrigger("0 55 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_CollectStravaActivitiesInEveryHour} timer trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.Strava_O_CollectStravaActivities, true);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.Strava_O_CollectStravaActivities}` with ID = `{instanceId}`.");
        }
    }
}
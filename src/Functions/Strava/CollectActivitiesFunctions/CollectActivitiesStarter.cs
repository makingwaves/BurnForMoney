using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.CollectActivitiesFunctions
{
    public static class CollectActivitiesStarter
    {
        // at *:55 every day
        [FunctionName(FunctionsNames.CollectStravaActivitiesInEvery20Minutes)]
        public static async Task RunTimerStarter([TimerTrigger("0 55 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.CollectStravaActivitiesInEvery20Minutes} timer trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, null);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CollectStravaActivities}` with ID = `{instanceId}`.");
        }
    }
}
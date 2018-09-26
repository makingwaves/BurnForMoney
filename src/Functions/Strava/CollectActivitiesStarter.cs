using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesStarter
    {
        [FunctionName(FunctionsNames.CollectStravaActivitiesInEvery20Minutes)]
        public static async Task RunTimerStarter([TimerTrigger("0 */30 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.CollectStravaActivitiesInEvery20Minutes} timer trigger processed a request at {DateTime.Now}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_CollectStravaActivities, null);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_CollectStravaActivities}` with ID = `{instanceId}`.");
        }
    }
}
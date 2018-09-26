using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesStarter
    {
        [FunctionName("CollectStravaActivitiesInEvery20Minutes")]
        public static async Task RunTimerStarter([TimerTrigger("0 */20 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"CollectStravaActivitiesInEvery20Minutes timer trigger processed a request at {DateTime.Now}.");

            var instanceId = await starter.StartNewAsync("O_CollectStravaActivities", null);
            log.LogInformation($"Started orchestration function: `O_CollectStravaActivities` with ID = `{instanceId}`.");
        }
    }
}
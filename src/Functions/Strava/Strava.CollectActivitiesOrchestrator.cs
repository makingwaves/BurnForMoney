using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesOrchestrator
    {
        [FunctionName("O_CollectStravaActivities")]
        public static async Task CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation("Orchestration function `CollectStravaActivities` received a request.");
            }

            var encryptedAccessTokens = await context.CallActivityAsync<string[]>("A_GetAccessTokens", null);
            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync("A_SaveSingleUserActivities", encryptedAccessTokens[i]);
            }

            await Task.WhenAll(tasks);
        }
    }
}

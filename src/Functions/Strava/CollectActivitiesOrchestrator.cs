using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.O_CollectStravaActivities)]
        public static async Task CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_CollectStravaActivities}` received a request.");
            }

            var encryptedAccessTokens = await context.CallActivityAsync<string[]>(FunctionsNames.A_GetAccessTokens, null);
            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync(FunctionsNames.A_SaveSingleUserActivities, encryptedAccessTokens[i]);
            }

            await Task.WhenAll(tasks);
        }
    }
}

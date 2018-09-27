using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesOrchestrator
    {
        private const string SystemName = "Strava";

        [FunctionName(FunctionsNames.O_CollectStravaActivities)]
        public static async Task CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_CollectStravaActivities}` received a request.");
            }

            var encryptedAccessTokens = await context.CallActivityAsync<string[]>(FunctionsNames.A_GetAccessTokens, null);
            var lastUpdate =
                await context.CallActivityAsync<DateTime?>(FunctionsNames.A_GetLastActivitiesUpdateDate, SystemName);

            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync(
                    FunctionsNames.A_SaveSingleUserActivities, 
                    (AccessToken: encryptedAccessTokens[i], LastUpdateDate: lastUpdate));
            }
            await Task.WhenAll(tasks);

            await context.CallActivityAsync(FunctionsNames.A_SetLastActivitiesUpdateDate,
                (SystemName: SystemName, LastUpdate: context.CurrentUtcDateTime));
        }
    }
}

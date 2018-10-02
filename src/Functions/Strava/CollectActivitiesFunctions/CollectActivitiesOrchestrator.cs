using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.CollectActivitiesFunctions
{
    public static class CollectActivitiesOrchestrator
    {
        private const string SystemName = "Strava";

        [FunctionName(FunctionsNames.O_CollectStravaActivities)]
        public static async Task CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            var optimize = context.GetInput<bool?>() ?? true;

            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_CollectStravaActivities}` received a request. {{optimize}}: {optimize}.");
            }

            // 1. Get all active access tokens
            var encryptedAccessTokens = await context.CallActivityAsync<string[]>(FunctionsNames.A_GetAccessTokens, null);
            if (encryptedAccessTokens.Length == 0)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] cannot find any active access tokens.");
                return;
            }
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] Retrieved {encryptedAccessTokens.Length} encrypted access tokens.");
            }

            // 2. Decrypted all access tokens
            var decryptionTasks = new Task<string>[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                decryptionTasks[i] = context.CallActivityAsync<string>(
                    FunctionsNames.A_DecryptAccessToken, encryptedAccessTokens[i]);
            }
            var decryptedAccessTokens = await Task.WhenAll(decryptionTasks);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] Decrypted {decryptedAccessTokens.Length} access tokens.");
            }

            var getActivitiesFrom = DateTime.UtcNow.AddMonths(-3);
            if (optimize)
            {
                // 3. Get time of the last update
                var lastUpdate = await context.CallActivityAsync<DateTime?>(FunctionsNames.A_GetLastActivitiesUpdateDate, SystemName);
                getActivitiesFrom = lastUpdate ?? getActivitiesFrom;
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] Retrieved time of the last update: {lastUpdate?.ToString() ?? "null"}.");
                }
            }

            // 4. Receive and store all new user activities
            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync(
                    FunctionsNames.A_RetrieveAndSaveSingleUserActivities, 
                    (AccessToken: decryptedAccessTokens[i], from: getActivitiesFrom));
            }
            await Task.WhenAll(tasks);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] Received and stored all new activities for {encryptedAccessTokens.Length} users.");
            }

            // 5. Set a new time of the last update
            await context.CallActivityAsync(FunctionsNames.A_SetLastActivitiesUpdateDate,
                (SystemName: SystemName, LastUpdate: context.CurrentUtcDateTime));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectStravaActivities}] Updated time of the last update [{context.CurrentUtcDateTime}].");
            }
        }
    }
}

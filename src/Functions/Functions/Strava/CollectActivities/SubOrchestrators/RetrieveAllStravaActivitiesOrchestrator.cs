using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.External.Strava.Api.Model;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators
{
    public static class RetrieveAllStravaActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.O_RetrieveAllStravaActivities)]
        public static async Task O_RetrieveAllStravaActivities(ILogger log,
            [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext,
            [Queue(QueueNames.PendingActivities)] CloudQueue pendingActivitiesQueue)
        {
            var (decryptedAccessTokens, startDate) = context.GetInput<(string[], DateTime)>();

            var tasks = new Task<List<StravaActivity>>[decryptedAccessTokens.Length];
            for (var i = 0; i < decryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync<List<StravaActivity>>(
                    FunctionsNames.A_RetrieveSingleUserActivities,
                    (AccessToken: decryptedAccessTokens[i], from: startDate));
            }

            foreach (var stravaActivity in (await Task.WhenAll(tasks)).SelectMany(_ => _))
            {
                var json = JsonConvert.SerializeObject(stravaActivity);
                var message = new CloudQueueMessage(json);
                await pendingActivitiesQueue.AddMessageAsync(message);
            }
        }
    }
}
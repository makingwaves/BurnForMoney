using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Functions.ActivityOperations.Processors;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public static class ProcessRawActivityFunc
    {
        private static readonly string[] SupportedSystems = { StravaActivityProcessor.System, ManualActivityProcessor.System };

        private static readonly IDictionary<string, IActivityProcessor> ActivityProcessors =
            new Dictionary<string, IActivityProcessor>
            {
                {StravaActivityProcessor.System, new StravaActivityProcessor()},
                {ManualActivityProcessor.System, new ManualActivityProcessor()}
            };

        [FunctionName(FunctionsNames.Q_ProcessRawActivity)]
        public static async Task ProcessNewActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.AddActivityRequests)] PendingRawActivity rawActivity,
            [Queue(QueueNames.PendingActivities)] CloudQueue pendingActivitiesQueue)
        {
            log.LogFunctionStart(FunctionsNames.Q_ProcessRawActivity);
            if (!SupportedSystems.Contains(rawActivity.Source))
            {
                throw new SystemNotSupportedException(rawActivity.Source);
            }

            var activityProcessor = ActivityProcessors[rawActivity.Source];

            var activity = activityProcessor.Process(rawActivity);

            var json = JsonConvert.SerializeObject(activity);
            await pendingActivitiesQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(FunctionsNames.Q_ProcessRawActivity);
        }
    }
}
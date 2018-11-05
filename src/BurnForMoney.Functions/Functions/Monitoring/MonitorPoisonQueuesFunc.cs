using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Functions.Monitoring
{
    public static class MonitorPoisonQueuesFunc
    {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient();
        
        [FunctionName("MonitorPoisonQueues")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var queuesToMonitor = new List<CloudQueue>();

            var configuration = ApplicationConfiguration.GetSettings(context);

            var queueStorageConnectionString = configuration.ConnectionStrings.AzureWebJobsStorage;
            var account = CloudStorageAccount.Parse(queueStorageConnectionString);
            var queueClient = account.CreateCloudQueueClient();

            QueueContinuationToken continuationToken = null;
            do
            {
                var segment = await queueClient.ListQueuesSegmentedAsync(continuationToken);
                var poisonQueues = segment.Results.Where(
                    q => q.Name.EndsWith("-poison", StringComparison.InvariantCultureIgnoreCase));
                queuesToMonitor.AddRange(poisonQueues);

                continuationToken = segment.ContinuationToken;
            }
            while (continuationToken != null);

            TelemetryConfiguration.Active.InstrumentationKey = configuration.ApplicationInsightsInstrumentationKey ?? string.Empty;
            foreach (var queue in queuesToMonitor)
            {
                await queue.FetchAttributesAsync();
                var queueLength = queue.ApproximateMessageCount;

                if (!configuration.IsLocalEnvironment)
                {
                    TelemetryClient.TrackMetric($"Poison queue length - {queue.Name}", (double)queueLength);
                }
                log.LogInformation($"Queue: {queue.Name} (Items: {queueLength})");
            }
        }
    }
}
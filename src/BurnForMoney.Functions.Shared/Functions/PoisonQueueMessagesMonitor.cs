using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Shared.Functions
{
    public static class PoisonQueueMessagesMonitor
    {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient();

        public static async Task RunAsync(string queueStorageConnectionString, string applicationInsightsInstrumentationKey, ILogger log, string systemPrefix = "BFM")
        {
            var queuesToMonitor = new List<CloudQueue>();

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

            TelemetryConfiguration.Active.InstrumentationKey = applicationInsightsInstrumentationKey ?? string.Empty;
            var trackMetric = !string.IsNullOrWhiteSpace(TelemetryConfiguration.Active.InstrumentationKey);

            var poisonMessages = 0;
            foreach (var queue in queuesToMonitor)
            {
                await queue.FetchAttributesAsync();
                var queueLength = queue.ApproximateMessageCount;

                if (trackMetric)
                {
                    TelemetryClient.TrackMetric($"[{systemPrefix}] Poison queue length - {queue.Name}", queueLength ?? 0);
                }
                log.LogInformation($"Queue: {queue.Name} (Items: {queueLength})");
                poisonMessages += queueLength ?? 0;
            }

            if (trackMetric)
            {
                TelemetryClient.TrackMetric($"[{systemPrefix}] Poison messages (overall) - ", poisonMessages);
            }
        }
    }
}
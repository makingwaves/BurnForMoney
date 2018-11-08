using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Strava.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions.Monitoring
{
    public static class MonitorPoisonQueuesFunc
    {
        [FunctionName("MonitorPoisonQueues")]
        public static async Task Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var configuration = ApplicationConfiguration.GetSettings(context);

            await PoisonQueueMessagesMonitor.RunAsync(configuration.ConnectionStrings.AzureWebJobsStorage,
                configuration.ApplicationInsightsInstrumentationKey, log, "BFM - Strava");
        }
    }
}
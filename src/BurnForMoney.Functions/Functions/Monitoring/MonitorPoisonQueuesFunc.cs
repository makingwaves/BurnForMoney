using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Monitoring
{
    public static class MonitorPoisonQueuesFunc
    {
        [FunctionName("MonitorPoisonQueues")]
        public static async Task Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogFunctionStart("MonitorPoisonQueues");
            var configuration = ApplicationConfiguration.GetSettings();

            await PoisonQueueMessagesMonitor.RunAsync(configuration.ConnectionStrings.AzureWebJobsStorage,
                configuration.ApplicationInsightsInstrumentationKey, log);
            log.LogFunctionEnd("MonitorPoisonQueues");
        }
    }
}
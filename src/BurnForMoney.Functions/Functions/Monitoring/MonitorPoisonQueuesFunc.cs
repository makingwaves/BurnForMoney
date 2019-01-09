using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Monitoring
{
    public static class MonitorPoisonQueuesFunc
    {
        [FunctionName(FunctionsNames.T_MonitorPoisonQueues)]
        public static async Task Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            await PoisonQueueMessagesMonitor.RunAsync(configuration.ConnectionStrings.AzureWebJobsStorage,
                configuration.ApplicationInsightsInstrumentationKey, log);
        }
    }
}
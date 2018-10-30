using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions;
using DurableTask.AzureStorage;
using DurableTask.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Support
{
    public static class DurableFunctionsHubOperations
    {
        [FunctionName(FunctionsNames.Support_PurgeDurableHubHistory)]
        [Disable] //Not supported by Durable Task (https://github.com/Azure/durabletask/blob/2c2e9c27980473641b99a81e4c35fb6245670590/src/DurableTask.AzureStorage/Tracking/AzureTableTrackingStore.cs)
        public static async Task<IActionResult> Support_PurgeDurableHubHistory([HttpTrigger(AuthorizationLevel.Admin, "delete", Route = "support/durablehub/purge/olderthan1day")]HttpRequest req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_PurgeDurableHubHistory} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(context);

            var settings = new AzureStorageOrchestrationServiceSettings
            {
                StorageConnectionString = configuration.ConnectionStrings.AzureWebJobsStorage,
                TaskHubName = starter.TaskHubName
            };

            var service = new AzureStorageOrchestrationService(settings);
            await service.PurgeOrchestrationHistoryAsync(DateTime.UtcNow.AddDays(-1), OrchestrationStateTimeRangeFilterType.OrchestrationLastUpdatedTimeFilter);

            return new OkObjectResult("Purge operation has been scheduled.");
        }
    }
}
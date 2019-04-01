//using System;
//using System.Threading.Tasks;
//using BurnForMoney.Functions.Shared.Extensions;
//using BurnForMoney.Functions.Shared.Functions.Extensions;
//using BurnForMoney.Functions.Strava.Configuration;
//using DurableTask.AzureStorage;
//using DurableTask.Core;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;
//
//namespace BurnForMoney.Functions.Strava.Functions._Support
//{
//    public static class DurableFunctionsHubOperationsFunc
//    {
//        [FunctionName(SupportFunctionsNames.PurgeDurableHubHistory)]
//        [Disable] //Not supported by Durable Task (https://github.com/Azure/durabletask/blob/2c2e9c27980473641b99a81e4c35fb6245670590/src/DurableTask.AzureStorage/Tracking/AzureTableTrackingStore.cs)
//        public static async Task<IActionResult> PurgeDurableHubHistory([HttpTrigger(AuthorizationLevel.Admin, "delete", Route = "support/durablehub/purge/olderthan1day")]HttpRequest req,
//            [OrchestrationClient]DurableOrchestrationClient starter,
//            ILogger log, [Configuration] ConfigurationRoot configuration)
//        {
//            var settings = new AzureStorageOrchestrationServiceSettings
//            {
//                StorageConnectionString = configuration.ConnectionStrings.AzureWebJobsStorage,
//                TaskHubName = starter.TaskHubName
//            };
//
//            var service = new AzureStorageOrchestrationService(settings);
//            await service.PurgeOrchestrationHistoryAsync(DateTime.UtcNow.AddDays(-1), OrchestrationStateTimeRangeFilterType.OrchestrationLastUpdatedTimeFilter);
//
//            return new OkObjectResult("Purge operation has been scheduled.");
//        }
//    }
//}
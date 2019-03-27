using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Activities
{
    public static class DeleteActivityFunc
    {
        [FunctionName(FunctionsNames.DeleteActivity)]
        public static async Task<IActionResult> DeleteActivity([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "athlete/{athleteId:guid}/activities/{activityId:guid}")] HttpRequest req,
            ExecutionContext executionContext, string athleteId, string activityId,
            ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            var athleteIdGuid = Guid.Parse(athleteId);
            var activityIdGuid = Guid.Parse(activityId);
           
            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = activityIdGuid, AthleteId = athleteIdGuid });
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            return new OkObjectResult("Request received.");
        }
    }
}
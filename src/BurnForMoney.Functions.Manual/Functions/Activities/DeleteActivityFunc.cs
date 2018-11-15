using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Manual.Functions.Activities
{
    public static class DeleteActivityFunc
    {
        [FunctionName(QueueNames.DeleteActivity)]
        public static async Task<IActionResult> DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "athlete/{athleteId:length(32)}/activities/{activityId:length(32)}")] HttpRequest req,
            ExecutionContext executionContext, string activityId,
            ILogger log,
            [Queue(AppQueueNames.DeleteActivityRequests)] CloudQueue outputQueue)
        {
            log.LogFunctionStart(QueueNames.DeleteActivity);

            var json = JsonConvert.SerializeObject(new DeleteActivityRequest { Id = activityId });
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(QueueNames.DeleteActivity);
            return new OkObjectResult("Request received.");
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class DeleteActivityFunc
    {
        [FunctionName(QueueNames.DeleteActivity)]
        public static async Task<IActionResult> DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "athlete/{athleteId:int:min(1)}/activities/{activityId:int:min(1)}")] HttpRequest req, 
            ExecutionContext executionContext, int activityId,
            ILogger log,
            [Queue(AppQueueNames.DeleteActivityRequests)] CloudQueue outputQueue)
        {
            log.LogFunctionStart(QueueNames.DeleteActivity);
            if (activityId <= 0)
            {
                return new BadRequestObjectResult($"{nameof(activityId)} must be greater than 0.");
            }

            await outputQueue.AddMessageAsync(new CloudQueueMessage(activityId.ToString()));
            log.LogFunctionEnd(QueueNames.DeleteActivity);
            return new OkObjectResult("Request received.");
        }
    }
}
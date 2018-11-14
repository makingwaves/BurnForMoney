using System;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class DeleteActivityFunc
    {
        [FunctionName("DeleteActivity")]
        public static IActionResult DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "athlete/{athleteId:int}/activities/{activityId:int}")] HttpRequest req, ExecutionContext executionContext, int activityId,
            [Queue(AppQueueNames.DeleteActivityRequests)] CloudQueue outputQueue)
        {
            throw new NotImplementedException();
        }
    }
}
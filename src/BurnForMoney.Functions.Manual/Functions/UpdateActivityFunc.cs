using System;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class UpdateActivityFunc
    {
        [FunctionName("UpdateActivity")]
        public static IActionResult UpdateActivity([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "athlete/{athleteId:int}/activities/{activityId:int}")] HttpRequest req, ExecutionContext executionContext,
            [Queue(AppQueueNames.UpdateActivityRequests)] CloudQueue outputQueue)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class AddActivityFunc
    {
        [FunctionName("AddActivity")]
        public static IActionResult AddActivity([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/{athleteId:int}/activities")] HttpRequest req, ExecutionContext executionContext,
            [Queue(AppQueueNames.AddActivityRequests)] CloudQueue outputQueue)
        {
            throw new NotImplementedException();
        }
    }
}
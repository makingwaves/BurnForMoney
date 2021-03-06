﻿using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.Shared.Extensions;
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
        public static async Task<IActionResult> DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "athlete/{athleteId:guid}/activities/{activityId:guid}")] HttpRequest req,
            ExecutionContext executionContext, string athleteId, string activityId,
            ILogger log,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = Guid.Parse(activityId), AthleteId = Guid.Parse(athleteId) });
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            return new OkObjectResult("Request received.");
        }
    }
}
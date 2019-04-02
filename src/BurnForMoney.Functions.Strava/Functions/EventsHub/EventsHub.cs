﻿using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class EventsHub
    {
        [FunctionName(FunctionsNames.EventsHub)]
        public static async Task<IActionResult> EventsHubAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "strava/events/hub")] HttpRequest req,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.StravaEvents)] CloudQueue outputQueue)
        {
            var eventData = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(eventData))
            {
                return new BadRequestResult();
            }

            await outputQueue.AddMessageAsync(new CloudQueueMessage(eventData));
            log.LogInformation(FunctionsNames.EventsHub, $"Added a message to queue: {QueueNames.StravaEvents}.");
            return new OkResult();
        }
    }
}
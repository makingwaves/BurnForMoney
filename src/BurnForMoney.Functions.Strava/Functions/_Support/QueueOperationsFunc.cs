using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Strava.Functions._Support
{
    public static class QueueOperationsFunc
    {
        [FunctionName(SupportFunctionsNames.ReprocessPoisonQueueMessages)]
        public static async Task<IActionResult> Support_ReprocessPoisonQueueMessages([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "support/reprocessQueueMessages/{queueName}")]HttpRequest req, ILogger log,
            [Queue("{queueName}")] CloudQueue queue,
            [Queue("{queueName}-poison")] CloudQueue poisonQueue, string queueName)
        {
            int.TryParse(req.Query["messageCount"], out var messageCountParameter);
            var messageCount = messageCountParameter == 0 ? 10 : messageCountParameter;

            var processedMessages = 0;
            while (processedMessages < messageCount)
            {
                var message = await poisonQueue.GetMessageAsync();
                if (message == null)
                    break;

                var messageId = message.Id;
                var popReceipt = message.PopReceipt;

                await queue.AddMessageAsync(message);
                await poisonQueue.DeleteMessageAsync(messageId, popReceipt);
                processedMessages++;
            }

            return new OkObjectResult($"Reprocessed {processedMessages} messages from the {poisonQueue.Name} queue.");
        }
    }
}
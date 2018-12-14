using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class AthleteOperationsFunc
    {
        [FunctionName(SupportFunctionsNames.DeactivateAthlete)]
        public static async Task<IActionResult> DeactivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:guid}/deactivate")]HttpRequest req, ILogger log,
            [Queue(AppQueueNames.DeactivateAthleteRequests)] CloudQueue deactivateAthleteRequestsQueue,
            string athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.DeactivateAthlete);

            var command = new DeactivateAthleteCommand
            {
                AthleteId = Guid.Parse(athleteId)
            };
            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await deactivateAthleteRequestsQueue.AddMessageAsync(message);

            log.LogFunctionEnd(SupportFunctionsNames.DeactivateAthlete);
            return new OkObjectResult("Request received.");
        }

        [FunctionName(SupportFunctionsNames.ActivateAthlete)]
        public static async Task<IActionResult> ActivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:guid}/activate")]HttpRequest req, ILogger log,
            [Queue(AppQueueNames.ActivateAthleteRequests)] CloudQueue activateAthleteRequestsQueue,
            string athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.ActivateAthlete);

            var command = new ActivateAthleteCommand
            {
                AthleteId = Guid.Parse(athleteId)
            };
            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await activateAthleteRequestsQueue.AddMessageAsync(message);

            log.LogFunctionEnd(SupportFunctionsNames.ActivateAthlete);
            return new OkObjectResult("Request received.");
        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Strava.Functions._Support
{
    public static class AthleteOperations
    {
        [FunctionName(SupportFunctionsNames.DeactivateAthlete)]
        public static async Task<IActionResult> DeactivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:guid}/deactivate")] HttpRequest req, ILogger log, string athleteId,
            [Queue(QueueNames.DeactivateAthleteRequests)] CloudQueue outputQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            await outputQueue.AddMessageAsync(new CloudQueueMessage(athleteId));

            return new OkObjectResult("Request received.");
        }

        [FunctionName(SupportFunctionsNames.ActivateAthlete)]
        public static async Task<IActionResult> ActivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:guid}/activate")] HttpRequest req, ILogger log, string athleteId,
            [Queue(QueueNames.ActivateAthleteRequests)] CloudQueue outputQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            await outputQueue.AddMessageAsync(new CloudQueueMessage(athleteId));

            return new OkObjectResult("Request received.");
        }
    }
}
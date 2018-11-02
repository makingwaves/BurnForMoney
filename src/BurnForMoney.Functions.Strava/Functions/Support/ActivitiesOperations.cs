using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Functions.Support
{
    public static class ActivitiesOperations
    {
        [FunctionName(FunctionsNames.Support_Strava_Activities_Collect)]
        public static async Task<IActionResult> Support_Strava_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/strava/athlete/{athleteId}/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, int athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Activities_Collect} function processed a request.");
            await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(athleteId.ToString()));
            return new OkResult();
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions._Support
{
    public static class ActivitiesOperationsFunc
    {
        [FunctionName(FunctionsNames.Support_Activities_Collect)]
        public static async Task<IActionResult> Support_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/strava/athlete/{athleteId}/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, int athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Activities_Collect} function processed a request.");

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?) null;

            var input = new CollectAthleteActivitiesInput
            {
                AthleteId = athleteId,
                From = from
            };
            var json = JsonConvert.SerializeObject(input);
            await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));
            return new OkObjectResult($"Ok. athleteId: {athleteId}, from: {from?.ToString() ?? "<null>"}");
        }
    }
}
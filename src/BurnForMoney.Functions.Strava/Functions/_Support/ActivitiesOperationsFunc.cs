using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Strava.Configuration;
using Dapper;
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
        [FunctionName(FunctionsNames.Support_Activities_All_Collect)]
        public static async Task<IActionResult> Support_Activities_All_Collect([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/all/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Support_Activities_Collect} function processed a request.");

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            IEnumerable<int> ids;

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                ids = await conn.QueryAsync<int>("SELECT Id FROM dbo.Athletes WHERE Active=1");
            }

            foreach (var athleteId in ids)
            {
                var input = new CollectAthleteActivitiesInput
                {
                    AthleteId = athleteId,
                    From = from
                };
                var json = JsonConvert.SerializeObject(input);
                await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));
            }

            return new OkResult();
        }

        [FunctionName(FunctionsNames.Support_Activities_Collect)]
        public static async Task<IActionResult> Support_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId}/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, int athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Activities_Collect} function processed a request.");

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;

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
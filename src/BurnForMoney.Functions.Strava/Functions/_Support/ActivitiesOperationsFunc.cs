using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Functions.CollectAthleteActivities.Dto;
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
            log.LogFunctionStart(FunctionsNames.Support_Activities_All_Collect);

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            IEnumerable<string> ids;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                ids = await conn.QueryAsync<string>("SELECT Id FROM dbo.Athletes WHERE Active=1");
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

            log.LogFunctionEnd(FunctionsNames.Support_Activities_All_Collect);
            return new OkResult();
        }

        [FunctionName(FunctionsNames.Support_Activities_Collect)]
        public static async Task<IActionResult> Support_CollectActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:length(32)}/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, string athleteId)
        {
            log.LogFunctionStart(FunctionsNames.Support_Activities_Collect);

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;

            var input = new CollectAthleteActivitiesInput
            {
                AthleteId = athleteId,
                From = from
            };
            var json = JsonConvert.SerializeObject(input);
            await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(FunctionsNames.Support_Activities_Collect);
            return new OkObjectResult($"Ok. athleteId: {athleteId}, from: {from?.ToString() ?? "<null>"}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Functions.CollectAthleteActivitiesFromStravaFunc.Dto;
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
    public static class PullAthleteActivitiesFunc
    {
        [FunctionName(SupportFunctionsNames.PullAllAthletesActivities)]
        public static async Task<IActionResult> PullAllAthletesActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/all/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(SupportFunctionsNames.PullAllAthletesActivities);

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;

            IEnumerable<Guid> ids;
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                ids = await conn.QueryAsync<Guid>("SELECT Id FROM dbo.Athletes WHERE Active=1");
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

            log.LogFunctionEnd(SupportFunctionsNames.PullAllAthletesActivities);
            return new OkResult();
        }

        [FunctionName(SupportFunctionsNames.PullAthleteActivities)]
        public static async Task<IActionResult> PullAthleteActivities([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:guid}/activities/collect")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues, string athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.PullAthleteActivities);

            var from = DateTime.TryParse(req.Query["from"], out var date) ? date : (DateTime?)null;

            var input = new CollectAthleteActivitiesInput
            {
                AthleteId = Guid.Parse(athleteId),
                From = from
            };
            var json = JsonConvert.SerializeObject(input);
            await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(SupportFunctionsNames.PullAthleteActivities);
            return new OkObjectResult($"Ok. athleteId: {athleteId}, from: {from?.ToString() ?? "<null>"}");
        }
    }
}
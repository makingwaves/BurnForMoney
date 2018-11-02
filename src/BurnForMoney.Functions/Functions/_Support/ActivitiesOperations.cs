using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class ActivitiesOperations
    {
        [FunctionName(FunctionsNames.Support_Activities_CollectMonthlyStatistics)]
        public static async Task<IActionResult> Support_Strava_Activities_MonthlyStatisticsCollect([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/activities/collectmonthlystatistics/{year}/{month}")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, int year, int month)
        {
            log.LogInformation($"{FunctionsNames.Support_Activities_CollectMonthlyStatistics} function processed a request.");

            if (month < 1 || month > 12)
            {
                const string errorMessage = "Function invoked with incorrect parameters. [month] must be in the range [1, 12].";
                log.LogWarning(errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }

            if (year < 2018)
            {
                const string errorMessage = "Function invoked with incorrect parameters. [year] must be greater or equal to 2018.";
                log.LogWarning(errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }

            var request = new CalculateMonthlyResultsRequest
            {
                Month = month,
                Year = year
            };

            var json = JsonConvert.SerializeObject(request);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogInformation($"{FunctionsNames.T_CalculateMonthlyAthleteResultsFromPreviousMonth} Put a message to the queue `{request.Month / request.Year}`.");

            return new OkResult();
        }

        [FunctionName(FunctionsNames.Support_Activities_Add)]
        public static async Task<IActionResult> Support_Activities_Add([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/strava/activities/add")]HttpRequest req, ILogger log,
            ExecutionContext executionContext, [Queue(QueueNames.PendingRawActivities)] CloudQueue queue)
        {
            log.LogInformation($"{FunctionsNames.Support_Activities_Add} function processed a request.");

            var data = await req.ReadAsStringAsync();
            try
            {
                JsonConvert.DeserializeObject<PendingRawActivity>(data,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error
                    }); // validate structure
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Provided input is in the incorrect format. {ex.Message}");
            }
            await queue.AddMessageAsync(new CloudQueueMessage(data));
            return new OkResult();
        }
    }
}
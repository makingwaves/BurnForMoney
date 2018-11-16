using System.Threading.Tasks;
using BurnForMoney.Functions.Functions.ResultsSnapshots.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class ActivitiesOperationsFunc
    {
        [FunctionName(SupportFunctionsNames.CollectMonthlyStatistics)]
        public static async Task<IActionResult> CollectMonthlyStatistics([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/activities/collectmonthlystatistics/{year:int:min(2018)}/{month:range(1,12)}")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, int year, int month)
        {
            log.LogFunctionStart(SupportFunctionsNames.CollectMonthlyStatistics);

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
            log.LogInformation(SupportFunctionsNames.CollectMonthlyStatistics, $"Put a message to the queue `{request.Month} / {request.Year}`.");

            log.LogFunctionEnd(SupportFunctionsNames.CollectMonthlyStatistics);
            return new OkResult();
        }
    }
}
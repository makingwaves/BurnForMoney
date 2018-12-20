using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.ReadModel.Functions.ResultsSnapshots.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace BurnForMoney.Functions.ReadModel.Functions.ResultsSnapshots
{
    public static class CalculateMonthlyAthleteResultsStarter
    {
        public const string FUNCTIONNAME_T_CalculateMonthlyAthleteResults =
            "T_CalculateMonthlyAthleteResults";

        public const string FUNCTIONNAME_T_CalculateMonthlyAthleteResultsFromPreviousMonth =
            "T_CalculateMonthlyAthleteResultsFromPreviousMonth";

        // every hour
        [FunctionName(FUNCTIONNAME_T_CalculateMonthlyAthleteResults)]
        public static async Task CalculateMonthlyAthleteResults([TimerTrigger("0 0 * * * *")]TimerInfo timer, [Queue(QueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, ILogger log, ExecutionContext context)
        {
            log.LogFunctionStart(FUNCTIONNAME_T_CalculateMonthlyAthleteResults);

            var date = DateTime.UtcNow;
            var request = new CalculateMonthlyResultsRequest
            {
                Month = date.Month,
                Year = date.Year
            };
            var json = JsonConvert.SerializeObject(request);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogInformation(FUNCTIONNAME_T_CalculateMonthlyAthleteResults, $"Put a message to the queue `{request.Month} / {request.Year}`.");
            log.LogFunctionEnd(FUNCTIONNAME_T_CalculateMonthlyAthleteResults);
        }

        // first day of the month at 1:00
        [FunctionName(FUNCTIONNAME_T_CalculateMonthlyAthleteResultsFromPreviousMonth)]
        public static async Task CalculateMonthlyAthleteResultsFromPreviousMonth([TimerTrigger("0 0 1 1 * *")]TimerInfo timer, [Queue(QueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, ILogger log, ExecutionContext context)
        {
            log.LogFunctionStart(FUNCTIONNAME_T_CalculateMonthlyAthleteResultsFromPreviousMonth);

            var date = DateTime.UtcNow.AddMonths(-1);
            var request = new CalculateMonthlyResultsRequest
            {
                Month = date.Month,
                Year = date.Year
            };
            var json = JsonConvert.SerializeObject(request);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogInformation(FUNCTIONNAME_T_CalculateMonthlyAthleteResultsFromPreviousMonth, $"Put a message to the queue `{request.Month} / {request.Year}`.");
            log.LogFunctionEnd(FUNCTIONNAME_T_CalculateMonthlyAthleteResultsFromPreviousMonth);
        }
    }

    public class QueueNames
    {
        public const string CalculateMonthlyResults = "calculate-monthly-results";
    }
}
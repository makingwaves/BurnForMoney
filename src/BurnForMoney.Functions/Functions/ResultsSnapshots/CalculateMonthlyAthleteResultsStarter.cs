using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Functions.ResultsSnapshots.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.ResultsSnapshots
{
    public static class CalculateMonthlyAthleteResultsStarter
    {
        // every hour
        [FunctionName(FunctionsNames.T_CalculateMonthlyAthleteResults)]
        public static async Task CalculateMonthlyAthleteResults([TimerTrigger("0 0 * * * *")]TimerInfo timer, [Queue(AppQueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, ILogger log, ExecutionContext context)
        {
            log.LogFunctionStart(FunctionsNames.T_CalculateMonthlyAthleteResults);

            var date = DateTime.UtcNow;
            var request = new CalculateMonthlyResultsRequest
            {
                Month = date.Month,
                Year = date.Year
            };
            var json = JsonConvert.SerializeObject(request);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogInformation(FunctionsNames.T_CalculateMonthlyAthleteResults, $"Put a message to the queue `{request.Month} / {request.Year}`.");
            log.LogFunctionEnd(FunctionsNames.T_CalculateMonthlyAthleteResults);
        }

        // first day of the month at 1:00
        [FunctionName(FunctionsNames.T_CalculateMonthlyAthleteResultsFromPreviousMonth)]
        public static async Task CalculateMonthlyAthleteResultsFromPreviousMonth([TimerTrigger("0 0 1 1 * *")]TimerInfo timer, [Queue(AppQueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, ILogger log, ExecutionContext context)
        {
            log.LogFunctionStart(FunctionsNames.T_CalculateMonthlyAthleteResultsFromPreviousMonth);

            var date = DateTime.UtcNow.AddMonths(-1);
            var request = new CalculateMonthlyResultsRequest
            {
                Month = date.Month,
                Year = date.Year
            };
            var json = JsonConvert.SerializeObject(request);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogInformation(FunctionsNames.T_CalculateMonthlyAthleteResultsFromPreviousMonth, $"Put a message to the queue `{request.Month} / {request.Year}`.");
            log.LogFunctionEnd(FunctionsNames.T_CalculateMonthlyAthleteResultsFromPreviousMonth);
        }
    }
}
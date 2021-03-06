﻿using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Configuration;
using BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots;
using BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.Dto;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Presentation.Functions._Support
{
    public static class ActivitiesOperationsFunc
    {
        public const string FUNCTIONNAME_CollectMonthlyStatistics =
            SupportFunctionNameConvention.Prefix + "CollectMonthlyStatistics";

        [FunctionName(FUNCTIONNAME_CollectMonthlyStatistics)]
        public static async Task<IActionResult> CollectMonthlyStatistics([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/activities/collectmonthlystatistics/{year:int:min(2018)}/{month:range(1,12)}")]HttpRequest req, ILogger log,
            [Queue(QueueNames.CalculateMonthlyResults)] CloudQueue outputQueue, int year, int month)
        {
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
            log.LogInformation(FUNCTIONNAME_CollectMonthlyStatistics, $"Put a message to the queue `{request.Month} / {request.Year}`.");
            return new OkResult();
        }
    }
}
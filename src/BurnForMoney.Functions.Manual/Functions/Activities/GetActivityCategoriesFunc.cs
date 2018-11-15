using System;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Manual.Functions.Activities
{
    public static class GetActivityCategoriesFunc
    {
        private static readonly string[] ActivityCategories = Enum.GetNames(typeof(ActivityCategory));

        [FunctionName(QueueNames.GetActivityCategories)]
        public static IActionResult GetActivityCategories([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/categories")] HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart(QueueNames.GetActivityCategories);
            log.LogFunctionEnd(QueueNames.GetActivityCategories);
            return new OkObjectResult(ActivityCategories);
        }
    }
}
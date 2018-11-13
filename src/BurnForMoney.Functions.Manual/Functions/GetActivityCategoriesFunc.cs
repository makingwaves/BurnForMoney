using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class GetActivityCategoriesFunc
    {
        [FunctionName("GetActivityCategories")]
        public static IActionResult GetActivityCategories([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/categories")] HttpRequest req, ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}
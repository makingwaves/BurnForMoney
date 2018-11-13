using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class DeleteActivityFunc
    {
        [FunctionName("DeleteActivity")]
        public static IActionResult DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "activities/{activityId:int}")] HttpRequest req, ExecutionContext executionContext, int activityId)
        {
            throw new NotImplementedException();
        }
    }
}
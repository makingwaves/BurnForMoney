using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class GetAthletesFunc
    {
        [FunctionName(QueueNames.GetAthletes)]
        public static IActionResult GetAthletes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athletes")] HttpRequest req, ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}
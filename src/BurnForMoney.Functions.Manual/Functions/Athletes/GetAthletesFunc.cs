using System;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Manual.Functions.Athletes
{
    public static class GetAthletesFunc
    {
        [FunctionName(QueueNames.GetAthletes)]
        public static IActionResult GetAthletes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athletes")] HttpRequest req, ExecutionContext executionContext,
            ILogger log)
        {
            log.LogFunctionStart(QueueNames.GetAthletes);

            throw new NotImplementedException();

            log.LogFunctionEnd(QueueNames.GetAthletes);
        }
    }
}
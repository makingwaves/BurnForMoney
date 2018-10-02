using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Public
{
    public static class TotalNumberApi
    {
        [FunctionName("TotalNumbers")]
        public static IActionResult TotalNumbers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "totalnumbers")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            var result = new
            {
                Distance = 792,
                Time = 263,
                Money = 17300.00,
                NumberOfTrainings=2246,
                PercentOfEngagedEmployees=27,
                MoneyEarnedThisMonth = 2400,
                MostFrequentActivities = new [] 
                {
                    new {
                        Category = "Ride",
                        Points = 250
                    },
                    new {
                        Category = "Running",
                        Points = 210
                    },
                    new {
                        Category = "Gym",
                        Points = 57
                    },
                    new {
                        Category = "Team sports",
                        Points = 55
                    },
                    new {
                        Category = "Fitness",
                        Points = 10
                    }
                }
            };

            return new OkObjectResult(result);
        }
    }
}
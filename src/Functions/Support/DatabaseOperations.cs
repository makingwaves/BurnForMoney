using System.Reflection;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Support.Logging;
using DbUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Support
{
    public static class DatabaseOperations
    {
        private static ConfigurationRoot _configuration;

        [FunctionName(FunctionsNames.Support_InitializeDatabase)]
        public static IActionResult RunInitializeDatabase([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/intializedatabase")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Support_InitializeDatabase} function processed a request.");

            _configuration = _configuration ?? ApplicationConfiguration.GetSettings(context);

            var logger = new DbUpLogger(log);
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(_configuration.ConnectionStrings.SqlDbConnectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogTo(logger)
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                return new BadRequestObjectResult(result.Error);
            }

            return new OkObjectResult("SQL database hase been bootstrapped successfully.");
        }
    }
}
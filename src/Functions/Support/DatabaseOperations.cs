﻿using System.Reflection;
using BurnForMoney.Functions.Configuration;
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
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();

        [FunctionName("InitializeDatabase")]
        public static IActionResult RunInitializeDatabase([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/intializedatabase")]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("InitializeDatabase function processed a request.");

            var settings = Configuration.GetSettings(context);

            var logger = new DbUpLogger(log);
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(settings.ConnectionStrings.SqlDbConnectionString)
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
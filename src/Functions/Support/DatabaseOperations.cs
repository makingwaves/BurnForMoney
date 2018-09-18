using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Support
{
    public static class DatabaseOperations
    {
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();

        [FunctionName("InitializeDatabase")]
        public static async Task<IActionResult> RunInitializeDatabase([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/intializedatabase")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("InitializeDatabase function processed a request.");

            var settings = Configuration.GetSettings(context);
            var repository = new AthleteRepository(settings.ConnectionStrings.SqlDbConnectionString, log);
            await repository.BootstrapAsync().ConfigureAwait(false);

            return new OkObjectResult("SQL database hase been bootstrapped successfully.");
        }
    }
}
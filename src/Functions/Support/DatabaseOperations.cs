using System.Linq;
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
            var repositories = new IRepository[]
            {
                new AthleteRepository(settings.ConnectionStrings.SqlDbConnectionString, log),
                new ActivityRepository(settings.ConnectionStrings.SqlDbConnectionString, log)
            };
            await Task.WhenAll(repositories.Select(r => r.BootstrapAsync())).ConfigureAwait(false);

            return new OkObjectResult("SQL database hase been bootstrapped successfully.");
        }
    }
}
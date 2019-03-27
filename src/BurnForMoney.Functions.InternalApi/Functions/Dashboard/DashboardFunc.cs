using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.InternalApi.Functions.Dashboard
{
    public class DashboardFunc
    {
        [FunctionName(FunctionsNames.GetDashboardTop)]
        public static async Task<IActionResult> GetDashboardTop([HttpTrigger(AuthorizationLevel.Function, "get", Route = "dashboardtop")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            int? month = null;
            int? year = null;
            var monthParameter = req.Query["month"];
            if (!string.IsNullOrWhiteSpace(monthParameter))
            {
                month = int.Parse(monthParameter);
            }
            var yearParameter = req.Query["year"];
            if (!string.IsNullOrWhiteSpace(yearParameter))
            {
                year = int.Parse(yearParameter);
            }

            var repository = new DashboardReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var dashboardTop = await repository.GetDashboardTopAsync(month, year);
       
            return new OkObjectResult(dashboardTop);
        }
    }
}
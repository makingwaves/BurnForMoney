using System.Threading.Tasks;
using BurnForMoney.Functions.Manual.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Manual.Functions.Athletes
{
    public static class GetAthletesFunc
    {
        [FunctionName(FunctionsNames.GetAthletes)]
        public static async Task<IActionResult> GetAthletes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athletes")] HttpRequest req, ExecutionContext executionContext,
            ILogger log)
        {
            log.LogFunctionStart(FunctionsNames.GetAthletes);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var athletes = conn.Query<AthleteDto>("SELECT Id, FirstName, LastName FROM dbo.Athletes WHERE Active=1");

                log.LogFunctionEnd(FunctionsNames.GetAthletes);
                return new OkObjectResult(athletes);
            }
        }
    }

    public class AthleteDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
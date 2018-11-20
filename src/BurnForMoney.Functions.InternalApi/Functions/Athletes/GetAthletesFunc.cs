using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class GetAthletesFunc
    {
        [FunctionName(FunctionsNames.GetAthletes)]
        public static async Task<IActionResult> GetAthletes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athletes")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.GetAthletes);

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
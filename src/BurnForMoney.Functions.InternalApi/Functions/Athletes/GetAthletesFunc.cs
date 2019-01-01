using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Repositories;
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
        public static async Task<IActionResult> GetAthletesAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athletes")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.GetAthletes);

            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athletes = await repository.GetAllActiveAsync();
       
            log.LogFunctionEnd(FunctionsNames.GetAthletes);
            return new OkObjectResult(athletes.Select(athlete => new {
                    athlete.Id,
                    athlete.FirstName,
                    athlete.LastName,
                    athlete.ProfilePictureUrl,
                    athlete.System
                }));
        }
    }
}
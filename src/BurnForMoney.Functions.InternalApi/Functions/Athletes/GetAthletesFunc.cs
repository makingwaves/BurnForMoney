using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Authorization;
using BurnForMoney.Infrastructure.Authorization.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
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
            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athletes = await repository.GetAllActiveAsync();
       
            return new OkObjectResult(athletes.Select(athlete => new {
                    athlete.Id,
                    athlete.FirstName,
                    athlete.LastName,
                    athlete.ProfilePictureUrl,
                    athlete.System
                }));
        }

        [FunctionName(FunctionsNames.GetCurrentAthelte)]
        public static async Task<IActionResult> GetCurrentAthelteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration,
            [BfmAuthorize] BfmPrincipal principal)
        {
            if(!principal.IsAuthenticated)
                return new StatusCodeResult(StatusCodes.Status403Forbidden);

            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athlete = await repository.GetAthleteByAadIdAsync(principal.AadId);

            if (!athlete.IsValid())
            {
                if(athlete == null)
                    return new StatusCodeResult(StatusCodes.Status404NotFound);

                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
            
            return new OkObjectResult(new {
                athlete.Id,
                athlete.FirstName,
                athlete.LastName,
                athlete.ProfilePictureUrl,
                athlete.System
            });
        }
    }
}
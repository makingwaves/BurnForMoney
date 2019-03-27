using System;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
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
        public static async Task<IActionResult> GetAthletesAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "athletes")] HttpRequest req,  
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

        [FunctionName(FunctionsNames.GetCurrentAthlete)]
        public static async Task<IActionResult> GetCurrentAthleteAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "athletes/{athleteId:guid}")] HttpRequest req,
            ILogger log, string athleteId, [Configuration] ConfigurationRoot configuration)
        {
            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athleteIdGuid = Guid.Parse(athleteId);
            var athlete = await repository.GetAthleteByIdAsync(athleteIdGuid);

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
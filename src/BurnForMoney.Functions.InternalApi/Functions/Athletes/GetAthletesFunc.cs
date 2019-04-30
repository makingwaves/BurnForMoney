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
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class GetAthletesFunc
    {
        [FunctionName(FunctionsNames.GetAthletes)]
        public static async Task<IActionResult> GetAthletesAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "athletes")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration, [Inject] IAthleteReadRepository repository)
        {
            var athletes = await repository.GetAllActiveAsync();
       
            return new OkObjectResult(athletes.Select(athlete => new {
                    athlete.Id,
                    athlete.FirstName,
                    athlete.LastName,
                    athlete.ProfilePictureUrl,
                    athlete.System
                }));
        }

        [FunctionName(FunctionsNames.GetAthlete)]
        public static async Task<IActionResult> GetAthleteAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "athletes/{id}")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration, string id, [Inject] IAthleteReadRepository repository)
        {
            string source = req.Query["source"];
            source = string.IsNullOrWhiteSpace(source) ? SourceNames.BurnForMoneySystem : source;

            AthleteRow athlete;
            try
            {
                athlete = await FetchAthlete(id, source, repository);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to fetch athlete. {ex.Message}");
            }

            if (athlete == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(new
            {
                athlete.Id,
                athlete.FirstName,
                athlete.LastName,
                athlete.ProfilePictureUrl,
                athlete.System
            });
        }

        private static async Task<AthleteRow> FetchAthlete(string id, string source, IAthleteReadRepository repository)
        {
            switch (source)
            {
                case SourceNames.BurnForMoneySystem:
                    return await repository.GetAthleteByIdAsync(Guid.Parse(id));
                case SourceNames.AzureActiveDirectory:
                    return await repository.GetAthleteByAadIdAsync(Guid.Parse(id));
                case SourceNames.Strava:
                    return await repository.GetAthleteByStravaIdAsync(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), "Invalid source specified");
            }
        }
    }
}
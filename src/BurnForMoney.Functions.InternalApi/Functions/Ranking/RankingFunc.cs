using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.InternalApi.Functions.Ranking
{
    public class RankingFunc
    {
        [FunctionName(FunctionsNames.GetTopAthletesForGivenActivityType)]
        public static async Task<IActionResult> GetTopAthletesForGivenActivityType([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ranking/{activityCategory?}")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration,
            string activityCategory)
        {
            int? month = null;
            int? year = null;
            int take = 10;
            var takeParameter = req.Query["take"];
            if (!string.IsNullOrWhiteSpace(takeParameter))
            {
                take = int.Parse(takeParameter);
            }
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

            var repository = new RankingReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var ranking = await repository.GetTopByPointsForCategoryAsync(activityCategory, take, month, year);
       
            return new OkObjectResult(ranking
                .Select(r => new RankingDto
                {
                    AthleteId = r.AthleteId,
                    AthleteFirstName = r.AthleteFirstName,
                    AthleteLastName = r.AthleteLastName,
                    Points = Convert.ToInt32(r.Points),
                    ProfilePictureUrl = r.ProfilePictureUrl
                }));
        }
    }

    public class RankingDto
    {
        public Guid AthleteId { get; set; }
        public string AthleteFirstName { get; set; }
        public string AthleteLastName { get; set; }
        public int Points { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
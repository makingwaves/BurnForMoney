using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.InternalApi.Functions.Ranking
{
    public class RankingFunc
    {
        private static List<RankingDto> Ranking = new List<RankingDto>
        {
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "John a strong man",
                AthleteLastName = "Doe",
                Points = 16,
                ProfilePictureUrl = "https://pbs.twimg.com/profile_images/960912417856413697/x2n_KYpB_400x400.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "John Runner",
                AthleteLastName = "Doe",
                Points = 26,
                ProfilePictureUrl = "https://pbs.twimg.com/profile_images/960912417856413697/x2n_KYpB_400x400.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "John Cyclist",
                AthleteLastName = "Doe",
                Points = 85,
                ProfilePictureUrl = "https://pbs.twimg.com/profile_images/960912417856413697/x2n_KYpB_400x400.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "Unknown 1",
                AthleteLastName = "Athlete",
                Points = 19,
                ProfilePictureUrl = "https://pbs.twimg.com/profile_images/960912417856413697/x2n_KYpB_400x400.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "Unknown 2",
                AthleteLastName = "Athlete",
                Points = 5,
                ProfilePictureUrl = "https://pbs.twimg.com/profile_images/960912417856413697/x2n_KYpB_400x400.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "Unknown 3",
                AthleteLastName = "Athlete",
                Points = 111,
                ProfilePictureUrl = "https://invalid.beprepared.jpg"
            },
            new RankingDto
            {
                AthleteId = Guid.NewGuid(),
                AthleteFirstName = "Unknown 4",
                AthleteLastName = "Athlete",
                Points = 85,
                ProfilePictureUrl = null
            }
        };

        [FunctionName(FunctionsNames.GetTopAthletesForGivenActivityType)]
        public static IActionResult GetTopAthletesForGivenActivityType([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ranking/{activityCategory?}")] HttpRequest req,  
            ILogger log, [Configuration] ConfigurationRoot configuration,
            string activityCategory)
        {
            int take = 10;
            var result = Enumerable.Empty<RankingDto>();

            if (string.IsNullOrWhiteSpace(activityCategory))
            {
                result = Ranking;
            }
            else
            {
                var activityCategoryEnum = (ActivityCategory)Enum.Parse(typeof(ActivityCategory), activityCategory);
                if (activityCategoryEnum == ActivityCategory.Run)
                {
                    result = new[] { Ranking[1] };
                }           
                if (activityCategoryEnum == ActivityCategory.Ride)
                {
                    result = new[] { Ranking[2] };
                }
                if (activityCategoryEnum == ActivityCategory.Fitness)
                {
                    result = new[] { Ranking[0] };
                }
                if (activityCategoryEnum == ActivityCategory.WinterSports)
                {
                    result = Ranking.Skip(3);
                }
            }

            var takeParameter = req.Query["take"];
            if (!string.IsNullOrWhiteSpace(takeParameter))
            {
                take = int.Parse(takeParameter);
            }

            return new OkObjectResult(result.Take(take).OrderByDescending(r => r.Points));
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
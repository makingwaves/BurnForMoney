using System;
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

namespace BurnForMoney.Functions.InternalApi.Functions.Activities
{
    public static class GetAthleteActivitiesFunc
    {
        [FunctionName(FunctionsNames.GetAthleteActivities)]
        public static async Task<IActionResult> GetAthleteActivitiesAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athlete/{athleteId:guid}/activities")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration, string athleteId)
        {
            var repository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activities = await repository.GetAthleteActivitiesAsync(Guid.Parse(athleteId), Source.None, DateTime.UtcNow.Month,
                DateTime.UtcNow.Year);

            return new OkObjectResult(activities
                .Select(activity => new {
                    activity.Id,
                    activity.ActivityType,
                    activity.DistanceInMeters,
                    activity.MovingTimeInMinutes,
                    activity.Source,
                    activity.StartDate
                }));
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Repositories;
using BurnForMoney.Infrastructure.Domain;
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
            log.LogFunctionStart(FunctionsNames.GetAthleteActivities);

            var repository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activities = await repository.GetAthleteActivitiesAsync(Guid.Parse(athleteId), Source.None, DateTime.UtcNow.Month,
                DateTime.UtcNow.Year);

            log.LogFunctionEnd(FunctionsNames.GetAthleteActivities);
            return new OkObjectResult(activities);
        }
    }
}
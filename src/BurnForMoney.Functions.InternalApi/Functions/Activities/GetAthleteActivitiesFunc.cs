using System;
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

namespace BurnForMoney.Functions.InternalApi.Functions.Activities
{
    public static class GetAthleteActivitiesFunc
    {
        [FunctionName(FunctionsNames.GetAthleteActivities)]
        public static async Task<IActionResult> GetAthleteActivities([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "athlete/{athleteId:length(32)}/activities")] HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration, string athleteId)
        {
            log.LogFunctionStart(FunctionsNames.GetAthleteActivities);

            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activities = conn.Query<ActivityDto>(@"SELECT Id, ActivityTime, Distance AS DistanceInMeters, MovingTime AS MovingTimeInMinutes, Category 
FROM dbo.Activities WHERE AthleteId=@AthleteId AND Source=@Source AND MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year",
                    new
                    {
                        AthleteId = athleteId,
                        Source = "Manual",
                        DateTime.UtcNow.Month,
                        DateTime.UtcNow.Year
                    });

                log.LogFunctionEnd(FunctionsNames.GetAthleteActivities);
                return new OkObjectResult(activities);
            }
        }
    }

    public class ActivityDto
    {
        public string Id { get; set; }
        public DateTime ActivityTime { get; set; }
        public string Category { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
    }
}
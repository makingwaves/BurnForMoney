using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.Strava.CollectActivities;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class SubmitAthleteActivity
    {
        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivities)] PendingActivity activity)
        {
            if (activity.System != "Strava")
            {
                throw new NotSupportedException($"System: {activity.System} is not supported.");
            }

            log.LogInformation($"{FunctionsNames.Q_SubmitAthleteActivity} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var activityCategory = StravaActivityMapper.MapToActivityCategory(activity.ActivityType);
                var points = PointsCalculator.Calculate(activityCategory, activity.DistanceInMeters, activity.MovingTimeInMinutes);
                var model = new Activity
                {
                    AthleteId = activity.AthleteId,
                    ActivityId = activity.ActivityId,
                    ActivityTime = activity.StartDate,
                    ActivityType = activity.ActivityType,
                    Distance = activity.DistanceInMeters,
                    MovingTime = activity.MovingTimeInMinutes,
                    Category = activityCategory.ToString(),
                    Points = points
                };

                var affectedRows = await conn.ExecuteAsync("Strava_Activity_Insert", model, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation($"Activity with id: {model.ActivityId} has been added.");
                }
                else
                {
                    log.LogWarning($"Failed to save activity with id: {model.ActivityId}.");
                }
            }
        }
    }
}
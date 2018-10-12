using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api.Model;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class SubmitAthleteActivity
    {
        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivities)] StravaActivity activity)
        {
            log.LogInformation($"{FunctionsNames.A_RetrieveSingleUserActivities} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var activityCategory = StravaActivityMapper.MapToActivityCategory(activity.Type);

                var model = new Activity
                {
                    AthleteId = activity.Athlete.Id,
                    ActivityId = activity.Id,
                    ActivityTime = activity.StartDate,
                    ActivityType = activity.Type.ToString(),
                    Distance = activity.Distance,
                    MovingTime = ToMinutes(activity.MovingTime),
                    Category = activityCategory.ToString()
                };
                model.Points = PointsCalculator.Calculate(activityCategory, model.Distance, model.MovingTime);

                var affectedRows = await conn.ExecuteAsync("Strava_Activity_Insert", model, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation($"Activity with id: {activity.Id} has been added.");
                }
                else
                {
                    log.LogWarning($"Failed to save activity with id: {activity.Id}.");
                }
            }
        }

        private static double ToMinutes(int seconds)
        {
            return Math.Round(seconds / 60.0, 2);
        }
    }
}
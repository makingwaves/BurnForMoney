using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions
{
    public static class SubmitAthleteActivity
    {
        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivities)] PendingActivity activity)
        {
            log.LogInformation($"{FunctionsNames.Q_SubmitAthleteActivity} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var model = new Activity
                {
                    AthleteId = activity.AthleteId,
                    ActivityId = activity.ActivityId,
                    ActivityTime = activity.StartDate,
                    ActivityType = activity.ActivityType,
                    Distance = activity.DistanceInMeters,
                    MovingTime = activity.MovingTimeInMinutes,
                    Category = activity.Category.ToString(),
                    Points = activity.Points
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
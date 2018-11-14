using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Functions.ActivityOperations.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public static class UpdateAthleteActivityFunc
    {
        [FunctionName(FunctionsNames.Q_UpdateAthleteActivity)]
        public static async Task Q_UpdateAthleteActivity(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivitiesUpdates)] PendingActivity activity)
        {
            log.LogFunctionStart(FunctionsNames.Q_SubmitAthleteActivity);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var model = new
                {
                    ActivityId = activity.SourceActivityId,
                    ActivityTime = activity.StartDate,
                    activity.ActivityType,
                    Distance = activity.DistanceInMeters,
                    MovingTime = activity.MovingTimeInMinutes,
                    Category = activity.Category.ToString(),
                    activity.Points
                };

                var sql = "UPDATE dbo.Activities SET ActivityTime=@ActivityTime, ActivityType=@ActivityType, Distance=@Distance, MovingTime=@MovingTime, Category=@Category, Points=@Points WHERE ActivityId=@ActivityId";
                var affectedRows = await conn.ExecuteAsync(sql, model)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation(FunctionsNames.Q_SubmitAthleteActivity, $"Activity with id: {model.ActivityId} has been updated.");
                }
                else
                {
                    log.LogError(FunctionsNames.Q_SubmitAthleteActivity, $"Failed to update activity with id: {model.ActivityId}.");
                    throw new FailedToUpdateActivityException(activity.SourceActivityId.ToString());
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_SubmitAthleteActivity);
        }
    }
}
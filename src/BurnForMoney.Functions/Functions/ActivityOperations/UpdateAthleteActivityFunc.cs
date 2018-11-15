using System.Collections.Concurrent;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Functions.ActivityOperations.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public static class UpdateAthleteActivityFunc
    {
        private static readonly ConcurrentDictionary<string, string> AthleteIdsMappings = new ConcurrentDictionary<string, string>();

        [FunctionName(FunctionsNames.Q_UpdateAthleteActivity)]
        public static async Task Q_UpdateAthleteActivity(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivitiesUpdates)] PendingActivity activity)
        {
            log.LogFunctionStart(FunctionsNames.Q_SubmitAthleteActivity);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var activityId = activity.Id;
                if (string.IsNullOrWhiteSpace(activityId))
                {
                    activityId = AthleteIdsMappings.GetOrAdd(activity.ExternalAthleteId,
                        await conn.QuerySingleOrDefaultAsync<string>("SELECT Id FROM dbo.Activities WHERE ExternalId=@ExternalId", new { activity.ExternalId }));

                    if (string.IsNullOrWhiteSpace(activityId))
                    {
                        throw new ActivityNotExistsException(activity.Id, activity.ExternalId);
                    }
                }

                var model = new
                {
                    Id = activityId,
                    activity.ExternalId,
                    ActivityTime = activity.StartDate,
                    activity.ActivityType,
                    Distance = activity.DistanceInMeters,
                    MovingTime = activity.MovingTimeInMinutes,
                    Category = activity.Category.ToString(),
                    activity.Points
                };

                const string sql = "UPDATE dbo.Activities SET ExternalId=@ExternalId, ActivityTime=@ActivityTime, ActivityType=@ActivityType, Distance=@Distance, MovingTime=@MovingTime, Category=@Category, Points=@Points WHERE Id=@Id";
                var affectedRows = await conn.ExecuteAsync(sql, model)
                    .ConfigureAwait(false);

                if (affectedRows == 1)
                {
                    log.LogInformation(FunctionsNames.Q_SubmitAthleteActivity, $"Activity with id: {model.Id} has been updated.");
                }
                else
                {
                    log.LogError(FunctionsNames.Q_SubmitAthleteActivity, $"Failed to update activity with id: {model.Id}.");
                    throw new FailedToUpdateActivityException(activity.Id);
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_SubmitAthleteActivity);
        }
    }
}
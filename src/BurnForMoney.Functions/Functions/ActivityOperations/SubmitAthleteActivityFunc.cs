using System.Collections.Concurrent;
using System.Data;
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
    public static class SubmitAthleteActivityFunc
    {
        private static readonly ConcurrentDictionary<int, int> AthleteIdsMappings = new ConcurrentDictionary<int, int>();

        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivities)] PendingActivity activity)
        {
            log.LogFunctionStart(FunctionsNames.Q_SubmitAthleteActivity);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var athleteId = activity.AthleteId;

                if (athleteId <= 0)
                {
                    athleteId = AthleteIdsMappings.GetOrAdd(activity.SourceAthleteId,
                        await conn.QuerySingleOrDefaultAsync<int>("SELECT Id FROM dbo.Athletes WHERE ExternalId=@SourceAthleteId", new { activity.SourceAthleteId }));

                    if (athleteId == 0)
                    {
                        throw new AthleteNotExistsException(activity.AthleteId, activity.SourceAthleteId);
                    }
                }

                var model = new
                {
                    AthleteId = athleteId,
                    ActivityId = activity.SourceActivityId,
                    ActivityTime = activity.StartDate,
                    activity.ActivityType,
                    Distance = activity.DistanceInMeters,
                    MovingTime = activity.MovingTimeInMinutes,
                    Category = activity.Category.ToString(),
                    activity.Points,
                    activity.Source
                };

                var affectedRows = await conn.ExecuteAsync("Activity_Insert", model, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation(FunctionsNames.Q_SubmitAthleteActivity, $"Activity with id: {model.ActivityId} has been added.");
                }
                else
                {
                    log.LogWarning(FunctionsNames.Q_SubmitAthleteActivity, $"Failed to save activity with id: {model.ActivityId}.");
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_SubmitAthleteActivity);
        }
    }
}
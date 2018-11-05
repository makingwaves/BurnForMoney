using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions
{
    public static class SubmitAthleteActivity
    {
        private static readonly ConcurrentDictionary<int, int> AthleteIdsMappings = new ConcurrentDictionary<int, int>();

        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(AppQueueNames.PendingActivities)] PendingActivity activity)
        {
            log.LogInformation($"{FunctionsNames.Q_SubmitAthleteActivity} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var athleteId = AthleteIdsMappings.GetOrAdd(activity.SourceAthleteId,
                    await conn.QuerySingleAsync<int>("SELECT Id FROM dbo.Athletes WHERE ExternalId=@SourceAthleteId", new {activity.SourceAthleteId}));

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
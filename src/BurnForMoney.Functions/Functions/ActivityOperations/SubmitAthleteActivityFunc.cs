using System.Collections.Concurrent;
using System.Data;
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
    public static class SubmitAthleteActivityFunc
    {
        private static readonly ConcurrentDictionary<string, string> AthleteIdsMappings = new ConcurrentDictionary<string, string>();

        [FunctionName(FunctionsNames.Q_SubmitAthleteActivity)]
        public static async Task Q_SubmitAthleteActivityAsync(ILogger log, ExecutionContext executionContext, [QueueTrigger(QueueNames.PendingActivities)] PendingActivity activity)
        {
            log.LogFunctionStart(FunctionsNames.Q_SubmitAthleteActivity);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var athleteId = activity.AthleteId;

                if (string.IsNullOrWhiteSpace(athleteId))
                {
                    athleteId = AthleteIdsMappings.GetOrAdd(activity.ExternalAthleteId,
                        await conn.QuerySingleOrDefaultAsync<string>("SELECT Id FROM dbo.Athletes WHERE ExternalId=@ExternalAthleteId", new { activity.ExternalAthleteId }));

                    if (string.IsNullOrWhiteSpace(athleteId))
                    {
                        throw new AthleteNotExistsException(activity.AthleteId, activity.ExternalAthleteId);
                    }
                }

                var model = new
                {
                    activity.Id,
                    AthleteId = athleteId,
                    activity.ExternalId,
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
                    log.LogInformation(FunctionsNames.Q_SubmitAthleteActivity, $"Activity with id: {model.Id} has been added.");
                }
                else
                {
                    log.LogWarning(FunctionsNames.Q_SubmitAthleteActivity, $"Failed to save activity with id: {model.Id}.");
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_SubmitAthleteActivity);
        }
    }
}
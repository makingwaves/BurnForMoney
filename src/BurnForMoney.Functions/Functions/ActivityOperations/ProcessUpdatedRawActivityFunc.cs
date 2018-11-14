using System.Threading.Tasks;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Functions.ActivityOperations.ActivityMappers;
using BurnForMoney.Functions.Functions.ActivityOperations.Dto;
using BurnForMoney.Functions.Functions.ActivityOperations.Points;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public class ProcessUpdatedRawActivityFunc
    {
        [FunctionName(FunctionsNames.Q_ProcessRawUpdatedActivity)]
        public static async Task ProcessUpdatedActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.UpdateActivityRequests)] PendingRawActivity rawActivity,
            [Queue(QueueNames.PendingActivitiesUpdates)] CloudQueue pendingActivityUpdatesQueue)
        {
            log.LogFunctionStart(FunctionsNames.Q_ProcessRawActivity);
            if (rawActivity.Source != "Strava")
            {
                throw new SystemNotSupportedException(rawActivity.Source);
            }

            var activityCategory = StravaActivityMapper.MapToActivityCategory(rawActivity.ActivityType);
            var points = PointsCalculator.Calculate(activityCategory, rawActivity.DistanceInMeters, rawActivity.MovingTimeInMinutes);

            var activity = new PendingActivity
            {
                SourceAthleteId = rawActivity.SourceAthleteId,
                SourceActivityId = rawActivity.SourceActivityId,
                StartDate = rawActivity.StartDate,
                ActivityType = rawActivity.ActivityType,
                DistanceInMeters = rawActivity.DistanceInMeters,
                MovingTimeInMinutes = rawActivity.MovingTimeInMinutes,
                Category = activityCategory,
                Points = points,
                Source = rawActivity.Source
            };

            var json = JsonConvert.SerializeObject(activity);
            await pendingActivityUpdatesQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(FunctionsNames.Q_ProcessRawActivity);
        }
    }
}
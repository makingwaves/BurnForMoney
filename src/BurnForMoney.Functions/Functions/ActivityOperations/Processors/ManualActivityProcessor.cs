using System;
using BurnForMoney.Functions.Functions.ActivityOperations.ActivityMappers;
using BurnForMoney.Functions.Functions.ActivityOperations.Dto;
using BurnForMoney.Functions.Functions.ActivityOperations.Points;
using BurnForMoney.Functions.Shared.Queues;

namespace BurnForMoney.Functions.Functions.ActivityOperations.Processors
{
    public class ManualActivityProcessor : IActivityProcessor
    {
        public const string System = "Manual";

        public bool CanProcess(PendingRawActivity raw) => raw.Source == System;

        public PendingActivity Process(PendingRawActivity raw)
        {
            if (!CanProcess(raw))
            {
                throw new InvalidOperationException();
            }

            var activityCategory = ManualActivityMapper.MapToActivityCategory(raw.ActivityType);
            var points = PointsCalculator.Calculate(activityCategory, raw.DistanceInMeters, raw.MovingTimeInMinutes);

            var activity = new PendingActivity
            {
                SourceAthleteId = raw.SourceAthleteId,
                SourceActivityId = raw.SourceActivityId,
                StartDate = raw.StartDate,
                ActivityType = raw.ActivityType,
                DistanceInMeters = raw.DistanceInMeters,
                MovingTimeInMinutes = raw.MovingTimeInMinutes,
                Category = activityCategory,
                Points = points,
                Source = raw.Source
            };
            return activity;
        }
    }
}
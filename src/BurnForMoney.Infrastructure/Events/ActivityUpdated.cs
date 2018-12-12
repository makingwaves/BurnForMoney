using System;

namespace BurnForMoney.Infrastructure.Events
{
    public class ActivityUpdated : DomainEvent
    {
        public readonly Guid ActivityId;
        public readonly double DistanceInMeters;
        public readonly double MovingTimeInMinutes;

        public readonly string ActivityType;
        public readonly DateTime StartDate;

        public ActivityUpdated(Guid activityId, double distanceInMeters, double movingTimeInMinutes, string activityType, DateTime startDate)
        {
            ActivityId = activityId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            StartDate = startDate;
        }
    }
}
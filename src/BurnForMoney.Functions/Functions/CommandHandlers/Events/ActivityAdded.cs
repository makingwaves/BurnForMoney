using System;
using BurnForMoney.Infrastructure;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public class ActivityAdded : DomainEvent
    {
        public readonly Guid ActivityId;
        public readonly string ExternalId;
        public readonly double DistanceInMeters;
        public readonly double MovingTimeInMinutes;
                
        public readonly string ActivityType;
        public readonly DateTime StartDate;
        public readonly string Source;

        public ActivityAdded(Guid activityId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, DateTime startDate, string source)
        {
            ActivityId = activityId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            StartDate = startDate;
            Source = source;
        }
    }
}
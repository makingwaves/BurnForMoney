using System;

namespace BurnForMoney.Infrastructure.Events
{
    public class ActivityAdded : DomainEvent
    {
        public readonly Guid ActivityId;
        public readonly Guid AthleteId;
        public readonly string ExternalId;
        public readonly double DistanceInMeters;
        public readonly double MovingTimeInMinutes;
                
        public readonly string ActivityType;
        public readonly DateTime StartDate;
        public readonly string Source;

        public ActivityAdded(Guid activityId, Guid athleteId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, DateTime startDate, string source)
        {
            ActivityId = activityId;
            AthleteId = athleteId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            StartDate = startDate;
            Source = source;
        }
    }
}
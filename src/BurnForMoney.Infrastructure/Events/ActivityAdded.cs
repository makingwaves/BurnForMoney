using System;
using BurnForMoney.Infrastructure.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly ActivityCategory ActivityCategory;
        public readonly DateTime StartDate;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly Source Source;

        public ActivityAdded(Guid activityId, Guid athleteId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory activityCategory, DateTime startDate, Source source)
        {
            ActivityId = activityId;
            AthleteId = athleteId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            ActivityCategory = activityCategory;
            StartDate = startDate;
            Source = source;
        }
    }
}
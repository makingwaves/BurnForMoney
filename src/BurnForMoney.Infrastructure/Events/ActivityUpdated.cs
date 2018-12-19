using System;
using BurnForMoney.Domain.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    public class ActivityUpdated : DomainEvent
    {
        public readonly Guid ActivityId;
        public readonly double DistanceInMeters;
        public readonly double MovingTimeInMinutes;

        public readonly string ActivityType;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly ActivityCategory ActivityCategory;
        public readonly DateTime StartDate;
        public readonly double Points;

        public ActivityUpdated(Guid activityId, double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory activityCategory, DateTime startDate, double points)
        {
            ActivityId = activityId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            ActivityCategory = activityCategory;
            StartDate = startDate;
            Points = points;
        }
    }
}
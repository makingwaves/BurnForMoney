using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActivityAdded : ActivityEvent
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
        public readonly double Points;

        public ActivityAdded(Guid activityId, Guid athleteId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory activityCategory, DateTime startDate, Source source, double points)
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
            Points = points;
        }
    }
}
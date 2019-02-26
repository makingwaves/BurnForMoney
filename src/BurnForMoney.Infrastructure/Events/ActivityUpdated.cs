using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActivityUpdated_V2 : ActivityEvent
    {
        public readonly Guid AthleteId;
        public readonly Guid ActivityId;
        public readonly double DistanceInMeters;
        public readonly double MovingTimeInMinutes;

        public readonly string ActivityType;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly ActivityCategory ActivityCategory;
        public readonly DateTime StartDate;
        public readonly double Points;

        public readonly PreviousActivityData PreviousData;

        public ActivityUpdated_V2(Guid athleteId, Guid activityId, double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory activityCategory, DateTime startDate, double points,
            PreviousActivityData previousData)
        {
            AthleteId = athleteId;
            ActivityId = activityId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            ActivityCategory = activityCategory;
            StartDate = startDate;
            Points = points;
            PreviousData = previousData;
        }

        public static ActivityUpdated_V2 ConvertFrom(ActivityUpdated @event)
        {
            return new ActivityUpdated_V2(Guid.Empty, @event.ActivityId, @event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType,
                @event.ActivityCategory, @event.StartDate, @event.Points, null);
        }
    }

    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActivityUpdated : ActivityEvent
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
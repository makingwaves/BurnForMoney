using System;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActivityDeleted_V2 : ActivityEvent
    {
        public readonly Guid AthleteId;
        public readonly Guid ActivityId;
        public readonly PreviousActivityData PreviousData;

        public ActivityDeleted_V2(Guid athleteId, Guid activityId, PreviousActivityData previousData)
        {
            AthleteId = athleteId;
            ActivityId = activityId;
            PreviousData = previousData;
        }

        public static ActivityDeleted_V2 ConvertFrom(ActivityDeleted @event)
        {
            return new ActivityDeleted_V2(Guid.Empty, @event.ActivityId, null);
        }
    }


    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActivityDeleted : ActivityEvent
    {
        public readonly Guid ActivityId;

        public ActivityDeleted(Guid activityId)
        {
            ActivityId = activityId;
        }
    }
}
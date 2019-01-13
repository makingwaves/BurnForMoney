using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
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
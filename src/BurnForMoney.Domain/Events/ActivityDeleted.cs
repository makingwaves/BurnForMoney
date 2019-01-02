using System;

namespace BurnForMoney.Domain.Events
{
    public class ActivityDeleted : DomainEvent
    {
        public readonly Guid ActivityId;

        public ActivityDeleted(Guid activityId)
        {
            ActivityId = activityId;
        }
    }
}
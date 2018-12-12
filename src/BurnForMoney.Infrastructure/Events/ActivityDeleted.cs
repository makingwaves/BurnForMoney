using System;

namespace BurnForMoney.Infrastructure.Events
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
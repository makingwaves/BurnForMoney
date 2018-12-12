using System;

namespace BurnForMoney.Infrastructure.Events
{
    public class ActivityDeleted : DomainEvent
    {
        public Guid ActivityId { get; set; }
    }
}
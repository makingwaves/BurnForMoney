using System;
using System.Collections.Generic;
using System.Linq;

namespace BurnForMoney.Infrastructure
{
    public abstract class DomainEvent
    {
        public string Name { get; }
        public DateTime TimeStamp { get; }
        public string SagaId { get; set; }
        public virtual string Version { get;} = "1.0";

        protected DomainEvent()
        {
            TimeStamp = DateTime.UtcNow;
            Name = GetType().FullName;
        }
    }

    public abstract class Aggregate : IAggregate
    {
        private readonly IList<DomainEvent> _uncommittedEvents = new List<DomainEvent>();

        public Guid Id { get; protected set; }
        Guid IAggregate.Id => Id;
        bool IAggregate.HasPendingChanges => _uncommittedEvents.Any();

        IEnumerable<DomainEvent> IAggregate.GetUncommittedEvents()
        {
            return _uncommittedEvents.ToArray();
        }

        void IAggregate.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        protected void RaiseEvent(DomainEvent @event)
        {
            _uncommittedEvents.Add(@event);
            //(this as dynamic).Apply((dynamic)@event);
        }
    }

}   
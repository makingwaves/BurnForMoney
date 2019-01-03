using System;
using System.Collections.Generic;
using System.Linq;

namespace BurnForMoney.Domain
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        public Guid Id { get; protected set; }

        public int Version { get; private set; }
        public int OriginalVersion { get; private set; }

        public bool HasPendingChanges => _changes.Any();

        public IEnumerable<DomainEvent> GetUncommittedEvents()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<DomainEvent> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        protected void ApplyChange(DomainEvent @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(DomainEvent @event, bool isNew)
        {
            ((dynamic)this).Apply((dynamic)@event);
            if (isNew)
            {
                _changes.Add(@event);
            }
            else
            {
                OriginalVersion++;
            }

            Version++;
        }

        public void Apply(DomainEvent e)
        {
            // no-op
        }
    }
}
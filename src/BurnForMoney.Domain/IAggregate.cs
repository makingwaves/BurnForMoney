using System;
using System.Collections.Generic;

namespace BurnForMoney.Domain
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
        bool HasPendingChanges { get; }
        IEnumerable<DomainEvent> GetUncommittedEvents();
        void MarkChangesAsCommitted();
        void LoadFromHistory(IEnumerable<DomainEvent> history);
    }
}
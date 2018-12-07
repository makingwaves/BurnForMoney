using System;
using System.Collections.Generic;

namespace BurnForMoney.Infrastructure
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
        bool HasPendingChanges { get; }
        IEnumerable<DomainEvent> GetUncommittedEvents();
        void MarkChangesAsCommitted();
        void LoadsFromHistory(IEnumerable<DomainEvent> history);
    }
}
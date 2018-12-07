using System;
using BurnForMoney.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace BurnForMoney.Domain
{
    public class Athlete : IAggregateRoot
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        public Guid Id { get; }

        public string ExternalId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string ProfilePictureUrl { get; }

        public bool IsActive { get; }

        public int Version { get; internal set; }

        public bool HasPendingChanges
        {
            get { return _changes.Any(); }
        }

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
    }
}

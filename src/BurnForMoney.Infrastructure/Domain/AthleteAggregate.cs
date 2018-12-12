using System;
using System.Collections.Generic;
using System.Linq;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Infrastructure.Domain
{
    public class Athlete : IAggregateRoot
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        public Guid Id { get; private set; }

        public string ExternalId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string ProfilePictureUrl { get; private set; }

        public bool IsActive { get; private set; }

        public System System { get; private set; }

        public int Version { get; private set; }

        public bool HasPendingChanges => _changes.Any();

        public IEnumerable<DomainEvent> GetUncommittedEvents()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void Apply(AthleteCreated @event)
        {
            Id = @event.Id;
            ExternalId = @event.ExternalId;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            ProfilePictureUrl = @event.ProfilePictureUrl;
            IsActive = true;
            System = @event.System;
        }

        public Athlete()
        {
            
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
            if (isNew) _changes.Add(@event);
            Version++;
        }

        public void Apply(DomainEvent e)
        {
            // no-op
        }

        public Athlete(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl, System system)
        {
            ApplyChange(new AthleteCreated(id, externalId, firstName, lastName, profilePictureUrl, system));
        }

        public void AddActivity(Guid id, string externalId, string activityType, double distanceInMeters, double movingTimeInMinutes, DateTime startDate, string source)
        {
            ApplyChange(new ActivityAdded(id, this.Id, externalId, distanceInMeters, movingTimeInMinutes, activityType, startDate, source));
        }
    }
}

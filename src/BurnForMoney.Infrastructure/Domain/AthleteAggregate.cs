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

        public int OriginalVersion { get; private set; }

        public List<Activity> Activities { get; } = new List<Activity>();

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

        public void Apply(AthleteDeactivated @event)
        {
            IsActive = false;
        }

        public void Apply(ActivityAdded @event)
        {
            Activities.Add(new Activity(@event.ActivityId, @event.ExternalId, @event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType, @event.StartDate, @event.Source));
        }

        public void Apply(ActivityUpdated @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            activity.Update(@event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType, @event.StartDate);
        }

        public void Apply(ActivityDeleted @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            Activities.Remove(activity);
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

        public Athlete(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl, System system)
        {
            ApplyChange(new AthleteCreated(id, externalId, firstName, lastName, profilePictureUrl, system));
        }

        public void AddActivity(Guid id, string externalId, string activityType, double distanceInMeters, double movingTimeInMinutes, DateTime startDate, string source)
        {
            if (Activities.Any(activity => activity.Id.Equals(id)))
            {
                throw new InvalidOperationException("Cannot add the same activity twice.");
            }

            ApplyChange(new ActivityAdded(id, this.Id, externalId, distanceInMeters, movingTimeInMinutes, activityType, startDate, source));
        }

        public void UpdateActivity(Guid id, string activityType, double distanceInMeters, double movingTimeInMinutes, DateTime startDate)
        {
            var activity = Activities.SingleOrDefault(a => a.Id == id);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {id} does not exists.");
            }

            if (activity.Id == id &&
                activity.ActivityType == activityType &&
                activity.ActivityType == activityType &&
                activity.DistanceInMeters == distanceInMeters &&
                activity.MovingTimeInMinutes == movingTimeInMinutes &&
                activity.StartDate == startDate)
            {
                throw new InvalidOperationException("Update operation must change at least one field. No changes detected.");
            }


            ApplyChange(new ActivityUpdated(id, distanceInMeters, movingTimeInMinutes, activityType, startDate));
        }

        public void DeleteActivity(Guid id)
        {
            var activity = Activities.SingleOrDefault(a => a.Id == id);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {id} does not exists.");
            }

            ApplyChange(new ActivityDeleted(id));
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is already deactivated.");
            }

            ApplyChange(new AthleteDeactivated(this.Id));
        }
    }

    public class Activity
    {
        public Guid Id { get; }
        public string ExternalId { get; }
        public double DistanceInMeters { get; private set; }
        public double MovingTimeInMinutes { get; private set; }

        public string ActivityType { get; private set; }
        public DateTime StartDate { get; private set; }
        public string Source { get; }

        public Activity(Guid activityId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, DateTime startDate, string source)
        {
            Id = activityId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            StartDate = startDate;
            Source = source;
        }

        public void Update(double distanceInMeters, double movingTimeInMinutes, string activityType, DateTime startDate)
        {
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            StartDate = startDate;
        }
    }
}

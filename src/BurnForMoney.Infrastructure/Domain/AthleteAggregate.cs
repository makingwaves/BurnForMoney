using System;
using System.Collections.Generic;
using System.Linq;
using BurnForMoney.Infrastructure.Domain.ActivityMappers;
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

        public Source Source { get; private set; }

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
            Source = @event.System;
        }

        public void Apply(AthleteDeactivated @event)
        {
            IsActive = false;
        }

        public void Apply(ActivityAdded @event)
        {
            Activities.Add(new Activity(@event.ActivityId, @event.ExternalId, @event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType, @event.ActivityCategory, @event.StartDate, @event.Source));
        }

        public void Apply(ActivityUpdated @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            activity.Update(@event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType, @event.ActivityCategory, @event.StartDate);
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

        public Athlete(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl, Source source)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            ApplyChange(new AthleteCreated(id, externalId, firstName, lastName, profilePictureUrl, source));
        }

        public void AddActivity(Guid id, string externalId, string activityType, double distanceInMeters, double movingTimeInMinutes, DateTime startDate, Source source)
        {
            if (string.IsNullOrWhiteSpace(activityType))
            {
                throw new ArgumentNullException(nameof(activityType));
            }
            if (distanceInMeters < 0)
            {
                throw new InvalidOperationException("Distance must be greater or equal to 0.");
            }
            if (movingTimeInMinutes <= 0)
            {
                throw new InvalidOperationException("Moving time must be greater than 0.");
            }
            if (startDate.Year < 2018)
            {
                throw new InvalidOperationException("Year must be greater than 2017.");
            }

            if (Activities.Any(activity => activity.Id.Equals(id)))
            {
                throw new InvalidOperationException("Cannot add the same activity twice.");
            }

            var category = MapToActivityCategory(activityType, source);

            ApplyChange(new ActivityAdded(id, this.Id, externalId, distanceInMeters, movingTimeInMinutes, activityType, category, startDate, source));
        }

        public void UpdateActivity(Guid id, string activityType, double distanceInMeters, double movingTimeInMinutes, DateTime startDate)
        {
            if (string.IsNullOrWhiteSpace(activityType))
            {
                throw new ArgumentNullException(nameof(activityType));
            }
            if (distanceInMeters < 0)
            {
                throw new InvalidOperationException("Distance must be greater or equal to 0.");
            }
            if (movingTimeInMinutes <= 0)
            {
                throw new InvalidOperationException("Moving time must be greater than 0.");
            }
            if (startDate.Year < 2018)
            {
                throw new InvalidOperationException("Year must be greater than 2017.");
            }

            var activity = Activities.SingleOrDefault(a => a.Id == id);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {id} does not exists.");
            }

            if (activity.Id == id &&
                activity.ActivityType == activityType &&
                activity.DistanceInMeters == distanceInMeters &&
                activity.MovingTimeInMinutes == movingTimeInMinutes &&
                activity.StartDate == startDate)
            {
                throw new InvalidOperationException("Update operation must change at least one field. No changes detected.");
            }

            var category = MapToActivityCategory(activityType, activity.Source);

            ApplyChange(new ActivityUpdated(id, distanceInMeters, movingTimeInMinutes, activityType, category, startDate));
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

        private ActivityCategory MapToActivityCategory(string activityType, Source source)
        {
            switch (source)
            {
                case Source.Strava:
                    return StravaActivityMapper.MapToActivityCategory(activityType);
                default:
                    return ManualActivityMapper.MapToActivityCategory(activityType);
            }
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
        public Source Source { get; }
        public ActivityCategory Category { get; private set; }

        public Activity(Guid activityId, string externalId, double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory category, DateTime startDate, Source source)
        {
            Id = activityId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            Category = category;
            StartDate = startDate;
            Source = source;
        }

        public void Update(double distanceInMeters, double movingTimeInMinutes, string activityType, ActivityCategory category, DateTime startDate)
        {
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            Category = category;
            StartDate = startDate;
        }
    }
}

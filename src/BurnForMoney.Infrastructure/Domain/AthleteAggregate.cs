using System;
using System.Collections.Generic;
using System.Linq;
using BurnForMoney.Domain.Domain.ActivityMappers;
using BurnForMoney.Domain.Events;

namespace BurnForMoney.Domain.Domain
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

        public void Apply(AthleteActivated @event)
        {
            IsActive = true;
        }

        public void Apply(ActivityAdded @event)
        {
            Activities.Add(new Activity(@event.ActivityId, @event.ExternalId, @event.DistanceInMeters,
                @event.MovingTimeInMinutes, @event.ActivityType, @event.ActivityCategory, @event.StartDate,
                @event.Source, @event.Points));
        }

        public void Apply(ActivityUpdated @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            activity.Update(@event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType,
                @event.ActivityCategory, @event.StartDate, @event.Points);
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
            ((dynamic) this).Apply((dynamic) @event);
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

        public Athlete(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl,
            Source source)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            ApplyChange(new AthleteCreated(id, externalId, firstName, lastName, profilePictureUrl, source));
        }

        public void AddActivity(Guid id, string externalId, string activityType, double distanceInMeters,
            double movingTimeInMinutes, DateTime startDate, Source source)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is deactivated.");
            }

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
            var points = PointsCalculator.Calculate(category, distanceInMeters, movingTimeInMinutes);

            ApplyChange(new ActivityAdded(id, Id, externalId, distanceInMeters, movingTimeInMinutes, activityType,
                category, startDate, source, points));
            ApplyChange(new PointsGranted(Id, points, PointsSource.Activity, id));
        }

        public void UpdateActivity(Guid id, string activityType, double distanceInMeters, double movingTimeInMinutes,
            DateTime startDate)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is deactivated.");
            }

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
                Math.Abs(activity.DistanceInMeters - distanceInMeters) < 0.05 &&
                Math.Abs(activity.MovingTimeInMinutes - movingTimeInMinutes) < 0.05 &&
                activity.StartDate == startDate)
            {
                throw new InvalidOperationException(
                    "Update operation must change at least one field. No changes detected.");
            }

            var category = MapToActivityCategory(activityType, activity.Source);
            var points = PointsCalculator.Calculate(category, distanceInMeters, movingTimeInMinutes);

            ApplyChange(new ActivityUpdated(id, distanceInMeters, movingTimeInMinutes, activityType, category,
                startDate, points));

            var originalPoints = Activities.Where(p => p.Id == id).Sum(l => l.Points);
            var pointsDelta = points - originalPoints;

            if (pointsDelta > 0)
            {
                ApplyChange(new PointsGranted(Id, pointsDelta, PointsSource.Activity, id));
            }

            if (pointsDelta < 0)
            {
                ApplyChange(new PointsLost(Id, pointsDelta, PointsSource.Activity, id));
            }
        }

        public void DeleteActivity(Guid id)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is deactivated.");
            }

            var activity = Activities.SingleOrDefault(a => a.Id == id);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {id} does not exists.");
            }

            ApplyChange(new ActivityDeleted(id));

            var originalPoints = Activities.Where(p => p.Id == id).Sum(l => l.Points);
            ApplyChange(new PointsLost(Id, originalPoints, PointsSource.Activity, id));
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new InvalidOperationException("Athlete is already activated.");
            }

            ApplyChange(new AthleteActivated(Id));
        }


        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is already deactivated.");
            }

            ApplyChange(new AthleteDeactivated(Id));
        }

        private static ActivityCategory MapToActivityCategory(string activityType, Source source)
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

        public double Points { get; set; }

        public Activity(Guid activityId, string externalId, double distanceInMeters, double movingTimeInMinutes,
            string activityType, ActivityCategory category, DateTime startDate, Source source, double points)
        {
            Id = activityId;
            ExternalId = externalId;
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            Category = category;
            StartDate = startDate;
            Source = source;
            Points = points;
        }

        public void Update(double distanceInMeters, double movingTimeInMinutes, string activityType,
            ActivityCategory category, DateTime startDate, double points)
        {
            DistanceInMeters = distanceInMeters;
            MovingTimeInMinutes = movingTimeInMinutes;
            ActivityType = activityType;
            Category = category;
            StartDate = startDate;
            Points = points;
        }
    }
}
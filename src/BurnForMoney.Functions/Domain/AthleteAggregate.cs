using System;
using System.Collections.Generic;
using System.Linq;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Domain.ActivityMappers;
using BurnForMoney.Functions.Exceptions;

namespace BurnForMoney.Functions.Domain
{
    public class Athlete : AggregateRoot
    {
        public Guid ActiveDirectoryId { get; set; }
        public string StravaId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string ProfilePictureUrl { get; private set; }

        public bool IsActive { get; private set; }
        

        public List<Activity> Activities { get; } = new List<Activity>();

        public void Apply(AthleteCreated @event)
        {
            Id = @event.Id;
            StravaId = @event.ExternalId;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            ProfilePictureUrl = @event.ProfilePictureUrl;
            IsActive = true;
        }

        public void Apply(AthleteCreated_V2 @event)
        {
            Id = @event.Id;
            ActiveDirectoryId = @event.AadId;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            ProfilePictureUrl = null;
            IsActive = true;
        }

        public void Apply(AthleteDeactivated @event)
        {
            IsActive = false;
        }

        public void Apply(AthleteActivated @event)
        {
            IsActive = true;
        }

        public void Apply(ActiveDirectoryIdAssigned @event)
        {
            ActiveDirectoryId = @event.ActiveDirectoryId;
        }

        public void Apply(ActivityAdded @event)
        {
            Activities.Add(new Activity(@event.ActivityId, @event.ExternalId, @event.DistanceInMeters,
                @event.MovingTimeInMinutes, @event.ActivityType, @event.ActivityCategory, @event.StartDate,
                @event.Source, @event.Points));
        }

        public void Apply(ActivityUpdated @event) => Apply(ActivityUpdated_V2.ConvertFrom(@event));
        public void Apply(ActivityUpdated_V2 @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            activity.Update(@event.DistanceInMeters, @event.MovingTimeInMinutes, @event.ActivityType,
                @event.ActivityCategory, @event.StartDate, @event.Points);
        }

        public void Apply(ActivityDeleted @event) => Apply(ActivityDeleted_V2.ConvertFrom(@event));
        public void Apply(ActivityDeleted_V2 @event)
        {
            var activity = Activities.Single(a => a.Id.Equals(@event.ActivityId));
            Activities.Remove(activity);
        }

        public void Apply(StravaIdAdded @event)
        {
            StravaId = @event.StravaId;
        }

        public Athlete()
        {}

        public Athlete(Guid id, Guid aadId, string firstName, string lastName)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (aadId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(aadId));
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            ApplyChange(new AthleteCreated_V2(id, aadId, firstName, lastName));
        }

        public void AddActivity(Guid id, string externalId, string activityType, double distanceInMeters,
            double movingTimeInMinutes, DateTime startDate, Source source)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(Id));
            }

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
                throw new InvalidOperationException($"Cannot add the same activity twice. Activity id: {id}");
            }

            if (Activities.Any(activity => activity.ExternalId.Equals(externalId) && activity.Source.Equals(source)))
            {
                throw new InvalidOperationException($"Cannot add the same activity twice. External id: [{externalId}].");
            }

            var category = MapToActivityCategory(activityType, source);
            var points = PointsCalculator.Calculate(category, distanceInMeters, movingTimeInMinutes);

            ApplyChange(new ActivityAdded(id, Id, externalId, distanceInMeters, movingTimeInMinutes, activityType,
                category, startDate, source, points));
        }

        public void UpdateActivity(Guid activityId, string activityType, double distanceInMeters, double movingTimeInMinutes,
            DateTime startDate)
        {
            if (activityId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(Id));
            }

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

            var activity = Activities.SingleOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {activityId} does not exists.");
            }

            if (activity.Id == activityId &&
                activity.ActivityType == activityType &&
                Math.Abs(activity.DistanceInMeters - distanceInMeters) < 0.05 &&
                Math.Abs(activity.MovingTimeInMinutes - movingTimeInMinutes) < 0.05 &&
                activity.StartDate == startDate)
            {
                throw new NoChangesDetectedException();
            }

            var category = MapToActivityCategory(activityType, activity.Source);
            var points = PointsCalculator.Calculate(category, distanceInMeters, movingTimeInMinutes);

            ApplyChange(new ActivityUpdated_V2(Id, activityId, distanceInMeters, movingTimeInMinutes, activityType, category, startDate, points, 
                new BurnForMoney.Infrastructure.Events.PreviousActivityData(activity.DistanceInMeters,
                    activity.MovingTimeInMinutes, activity.ActivityType, activity.Category, activity.StartDate, activity.Points)));
        }

        public void DeleteActivity(Guid activityId)
        {
            if (activityId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(Id));
            }

            if (!IsActive)
            {
                throw new InvalidOperationException("Athlete is deactivated.");
            }

            var activity = Activities.SingleOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with id: {activityId} does not exists.");
            }

            ApplyChange(new ActivityDeleted_V2(Id, activityId, new BurnForMoney.Infrastructure.Events.PreviousActivityData(activity.DistanceInMeters,
                activity.MovingTimeInMinutes, activity.ActivityType, activity.Category, activity.StartDate, activity.Points)));
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

        public void AddStravaAccount(string stravaId)
        {
            if (!string.IsNullOrEmpty(StravaId))
                throw new InvalidOperationException("Athlete has already a stravaId account.");
            
            ApplyChange(new StravaIdAdded
            {
                AthleteId = Id,
                StravaId = stravaId
            });
        }

        public void AssignActiveDirectoryId(Guid activeDirectoryId)
        {
            if (activeDirectoryId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(activeDirectoryId));
            }

            ApplyChange(new ActiveDirectoryIdAssigned(Id, activeDirectoryId));
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
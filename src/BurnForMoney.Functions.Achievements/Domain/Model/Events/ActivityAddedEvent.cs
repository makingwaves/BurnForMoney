using BurnForMoney.Infrastructure;

namespace BurnForMoney.Functions.Achievements.Domain.Model.Events
{
    public class ActivityAddedEvent : DomainEvent
    {
        public string AthleteId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }

    public class ActivityUpdatedEvent : DomainEvent
    {
        public string AthleteId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }

    public class ActivityDeletedEvent : DomainEvent
    {
        public string AthleteId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }
}
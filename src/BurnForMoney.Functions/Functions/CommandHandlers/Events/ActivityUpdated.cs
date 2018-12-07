using System;
using BurnForMoney.Infrastructure;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public class ActivityUpdated : DomainEvent
    {
        public string ActivityId { get; set; }
        public string ExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }

        public string ActivityType { get; set; }
        public DateTime StartDate { get; set; }
        public string Source { get; set; }
    }
}
using System;

namespace BurnForMoney.Functions.Shared.Queues
{
    public class PendingRawActivity
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }

        public string AthleteId { get; set; }
        public string ExternalAthleteId { get; set; }

        public DateTime StartDate { get; set; }
        public string ActivityType { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public string Source { get; set; }
    }
}
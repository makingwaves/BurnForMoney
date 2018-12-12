using System;

namespace BurnForMoney.Functions.Shared.Repositories
{
    public class ActivityRow : Row
    {
        public Guid AthleteId { get; set; }
        public string ExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }

        public string ActivityType { get; set; }
        public DateTime StartDate { get; set; }
        public string Source { get; set; }
    }
}
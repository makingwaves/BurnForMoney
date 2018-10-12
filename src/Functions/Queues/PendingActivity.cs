using System;

namespace BurnForMoney.Functions.Queues
{
    public class PendingActivity
    {
        public long ActivityId { get; set; }
        public int AthleteId { get; set; }
        public DateTime StartDate { get; set; }
        public string ActivityType { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public string System { get; set; }
    }
}
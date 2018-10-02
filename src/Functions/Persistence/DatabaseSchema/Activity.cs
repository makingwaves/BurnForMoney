using System;

namespace BurnForMoney.Functions.Persistence.DatabaseSchema
{
    public class Activity
    {
        public int AthleteId { get; set; }
        public long ActivityId { get; set; }
        public DateTime ActivityTime { get; set; }
        public string ActivityType { get; set; }
        public double Distance { get; set; }
        public double MovingTime { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }
}
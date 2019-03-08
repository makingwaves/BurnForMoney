using System;

namespace BurnForMoney.Functions.Presentation.Views.Poco
{
    public class Activity
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
        public string ExternalId { get; set; }
        public DateTime ActivityTime { get; set; }
        public string ActivityType { get; set; }
        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
        public double Points { get; set; }
    }
}
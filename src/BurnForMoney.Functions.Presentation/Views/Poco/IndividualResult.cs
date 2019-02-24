using System;

namespace BurnForMoney.Functions.Presentation.Views.Poco
{
    public class IndividualResult : IVersioned
    {
        public Guid AthleteId { get; set; }
        public string Category { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public double Points { get; set; }

        public int Version { get; set; }
    }
}
using System.Collections.Generic;

namespace BurnForMoney.Functions.Shared.Persistence
{
    public class AthleteMonthlyResult
    {
        public string AthleteId { get; set; }
        public string AthleteName { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
        public List<AthleteMonthlyResultActivity> Activities { get; set; }
    }
}
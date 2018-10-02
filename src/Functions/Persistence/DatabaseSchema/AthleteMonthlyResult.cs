using System.Collections.Generic;

namespace BurnForMoney.Functions.Persistence.DatabaseSchema
{
    public class AthleteMonthlyResult
    {
        public int AthleteId { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
        public List<AthleteMonthlyResultActivity> Activities { get; set; }
    }

    public class AthleteMonthlyResultActivity
    {
        public string Category { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
    }
}
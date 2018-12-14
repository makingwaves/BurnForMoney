using System;
using System.Collections.Generic;

namespace BurnForMoney.Functions.Shared.Persistence
{
    public class AthleteMonthlyResult
    {
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public List<AthleteResult> AthleteResults { get; set; }
    }

    public class AthleteResult
    {
        public Guid Id { get; set; }
        public string AthleteName { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
        public List<AthleteMonthlyResultActivity> Activities { get; set; }
    }
}
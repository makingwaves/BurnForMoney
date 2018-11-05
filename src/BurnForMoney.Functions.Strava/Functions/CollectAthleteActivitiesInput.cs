using System;

namespace BurnForMoney.Functions.Strava.Functions
{
    public class CollectAthleteActivitiesInput
    {
        public int AthleteId { get; set; }
        public DateTime? From { get; set; }
    }
}
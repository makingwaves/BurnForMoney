using System;

namespace BurnForMoney.Functions.Strava.Functions.CollectAthleteActivitiesFromStravaFunc.Dto
{
    public class CollectAthleteActivitiesInput
    {
        public Guid AthleteId { get; set; }
        public DateTime? From { get; set; }
    }
}
using System;

namespace BurnForMoney.Functions.Strava.Functions.CollectAthleteActivities.Dto
{
    public class CollectAthleteActivitiesInput
    {
        public string AthleteId { get; set; }
        public DateTime? From { get; set; }
    }
}
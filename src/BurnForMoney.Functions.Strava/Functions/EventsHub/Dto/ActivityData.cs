using System;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub.Dto
{
    public class ActivityData
    {
        public Guid AthleteId { get; set; }
        public string StravaAthleteId { get; set; }
        public string StravaActivityId { get; set; }
    }
}
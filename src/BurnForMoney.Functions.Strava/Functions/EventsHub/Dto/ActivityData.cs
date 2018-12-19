using System;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub.Dto
{
    public class ActivityData
    {
        public Guid AthleteId { get; set; }
        public string StravaActivityId { get; set; }

        public ActivityData(Guid athleteId, string stravaActivityId)
        {
            AthleteId = athleteId;
            StravaActivityId = stravaActivityId;
        }
    }
}
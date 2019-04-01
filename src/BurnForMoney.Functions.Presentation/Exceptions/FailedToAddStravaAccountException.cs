using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToAddStravaAccountException : DataException
    {
        public FailedToAddStravaAccountException(Guid athleteId, string stravaId)
            : base($"Failed to add a strava AthleteId: [{athleteId}], StravaId: [{stravaId}].")
        {}
    }
}
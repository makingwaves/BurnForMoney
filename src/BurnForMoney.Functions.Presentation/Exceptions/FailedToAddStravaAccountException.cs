using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToAddStravaAccountException : DataException
    {
        public FailedToAddStravaAccountException(Guid athleteId, string stravaId)
            : base($"Failed to add a strava AthleteId: [{athleteId}], StravaId: [{stravaId}].")
        {}

        protected FailedToAddStravaAccountException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
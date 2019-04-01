using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class FailedToApproveAthleteTimeoutException : TimeoutException
    {
        public FailedToApproveAthleteTimeoutException(Guid athleteId, string stravaId, DateTime dateTime)
            : base($"Failed to approve athlete: <{athleteId}, <{stravaId}> in the allotted time period: [{dateTime.ToString(CultureInfo.InvariantCulture)}].")
        {
        }

        protected FailedToApproveAthleteTimeoutException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
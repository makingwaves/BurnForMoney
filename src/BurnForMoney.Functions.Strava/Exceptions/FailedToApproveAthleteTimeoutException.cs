using System;
using System.Globalization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class FailedToApproveAthleteTimeoutException : TimeoutException
    {
        public FailedToApproveAthleteTimeoutException(string athleteId, string stravaId, DateTime dateTime)
            : base($"Failed to approve athlete: <{athleteId}, <{stravaId}> in the allotted time period: [{dateTime.ToString(CultureInfo.InvariantCulture)}].")
        {
        }
    }
}
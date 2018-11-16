using System;
using System.Globalization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class FailedToApproveAthleteTimeoutException : TimeoutException
    {
        public FailedToApproveAthleteTimeoutException(int athleteId, DateTime dateTime)
            : base($"Failed to approve athlete: <{athleteId}> in the allotted time period: [{dateTime.ToString(CultureInfo.InvariantCulture)}].")
        {
        }
    }
}
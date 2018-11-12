using System;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class AccessTokenNotFoundException : Exception
    {
        public AccessTokenNotFoundException(int athleteId)
            : base($"Cannot find an access token fot athlete: {athleteId}. Athlete might either be not verified or deleted or his token is invalid.")
        {
        }
    }
}
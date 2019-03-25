using System;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class StravaAccountExistsException : Exception
    {
        public StravaAccountExistsException()
        { }

        public StravaAccountExistsException(string stravaId)
            : base($"{nameof(StravaAccountExistsException)} Athlete account with strava id: [{stravaId}] already exists..")
        {}
    }
}
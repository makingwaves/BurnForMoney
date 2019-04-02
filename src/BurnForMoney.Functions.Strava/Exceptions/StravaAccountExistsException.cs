using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class StravaAccountExistsException : Exception
    {
        
        public StravaAccountExistsException(string stravaId)
            : base($"{nameof(StravaAccountExistsException)} Athlete account with strava id: [{stravaId}] already exists..")
        {}

        protected StravaAccountExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}
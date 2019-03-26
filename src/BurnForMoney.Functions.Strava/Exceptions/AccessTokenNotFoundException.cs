using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class AccessTokenNotFoundException : Exception
    {
        public AccessTokenNotFoundException(string athleteId)
            : base($"Cannot find an access token fot athlete: {athleteId}. Athlete might either be not verified or deleted or his token is invalid.")
        {
        }

        protected AccessTokenNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
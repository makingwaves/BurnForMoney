using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class FailedToAddAccessTokenException : DataException
    {
        public FailedToAddAccessTokenException(string athleteId)
            : base($"Failed to add an access token for athlete: [{athleteId}].")
        {
        }

        public FailedToAddAccessTokenException(string athleteId, Exception inner)
            : base($"Failed to add an access token for athlete: [{athleteId}].", inner)
        {
        }

        protected FailedToAddAccessTokenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
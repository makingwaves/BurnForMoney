using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class FailedToRefreshAccessTokenException : DataException
    {
        public FailedToRefreshAccessTokenException(string athleteId)
            : base($"Failed to refresh access token for athlete: [{athleteId}].")
        {
        }

        protected FailedToRefreshAccessTokenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
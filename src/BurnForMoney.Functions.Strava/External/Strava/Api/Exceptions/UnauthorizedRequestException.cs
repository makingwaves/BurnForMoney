using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions
{
    [Serializable]
    public class UnauthorizedRequestException : UnauthorizedAccessException
    {
        public UnauthorizedRequestException()
            : base("Application is not authorized to get athlete's data (expired / deauthorized / unauthorized).")
        {
        }

        protected UnauthorizedRequestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
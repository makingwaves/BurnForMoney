using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions
{
    [Serializable]
    public class ExceededRateLimitsException : UnauthorizedAccessException
    {
        public ExceededRateLimitsException(RateLimits rateLimits)
            : base("Application exceeded 15-minutes/daily rate limits. " +
                   $"Usage: {rateLimits.ShortTermUsage}, {rateLimits.DailyUsage}. " +
                   $"Limits: {rateLimits.ShortTermLimit}, {rateLimits.DailyLimit}.")
        {
        }

        protected ExceededRateLimitsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
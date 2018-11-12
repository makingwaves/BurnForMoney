using System;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions
{
    public class ExceededRateLimitsException : UnauthorizedAccessException
    {
        public ExceededRateLimitsException(RateLimits rateLimits)
            : base("Application exceeded 15-minutes/daily rate limits. " + 
                   $"Usage: {rateLimits.ShortTermUsage}, {rateLimits.DailyUsage}. " +
                   $"Limits: {rateLimits.ShortTermLimit}, {rateLimits.DailyLimit}.")
        {

        }
    }
}
using System;
using System.Linq;
using System.Net;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using RestSharp;

namespace BurnForMoney.Functions.Strava.External.Strava.Api
{
    public static class RestSharpExtensions
    {
        public static void ThrowExceptionIfNotSuccessful(this IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var rateLimits = response.GetRateLimits();

                if (!rateLimits.IsUsageWithinTheLimits())
                {
                    throw new ExceededRateLimitsException(rateLimits);
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedRequestException();
            }
            
            if (!response.IsSuccessful)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }
        }

        public static RateLimits GetRateLimits(this IRestResponse response)
        {
            const char limitsSeparator = ',';

            var headers = response.Headers.ToList();
            var rateLimit = headers.FirstOrDefault(h => h.Name == "X-RateLimit-Limit")?.Value.ToString();
            var rateUsage = headers.FirstOrDefault(h => h.Name == "X-RateLimit-Usage")?.Value.ToString();

            if (string.IsNullOrWhiteSpace(rateLimit) || string.IsNullOrWhiteSpace(rateUsage))
            {
                throw new InvalidOperationException("Rate limits are not included in the responze.");
            }

            var rateLimitsSplit = rateLimit.Split(limitsSeparator);
            var rateUsageSplit = rateUsage.Split(limitsSeparator);
            
            return new RateLimits
            {
                ShortTermLimit = int.Parse(rateLimitsSplit[0]),
                DailyLimit = int.Parse(rateLimitsSplit[1]),
                ShortTermUsage = int.Parse(rateUsageSplit[0]),
                DailyUsage = int.Parse(rateUsageSplit[1])
            };
        }
    }
}
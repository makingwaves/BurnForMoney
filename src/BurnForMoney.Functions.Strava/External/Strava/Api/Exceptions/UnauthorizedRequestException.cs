using System;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions
{
    public class UnauthorizedRequestException : UnauthorizedAccessException
    {
        public UnauthorizedRequestException()
            :base("Application is not authorized to get athlete's data (expired / deauthorized / unauthorized).")
        {
            
        }
    }
}
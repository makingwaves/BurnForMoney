using System;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions
{
    public class ActivityNotFoundException : Exception 
    {
        public ActivityNotFoundException(string id)
            : base($"ActivityRow with id: {id} does not exists.")
        {

        }
    }
}
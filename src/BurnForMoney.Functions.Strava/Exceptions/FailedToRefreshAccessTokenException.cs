﻿using System.Data;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class FailedToRefreshAccessTokenException : DataException
    {
        public FailedToRefreshAccessTokenException(int athleteId)
            : base($"Failed to refresh access token for athlete: [{athleteId}].")
        {
        }
    }
}
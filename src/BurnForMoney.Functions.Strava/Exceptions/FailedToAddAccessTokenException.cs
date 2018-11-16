using System.Data;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class FailedToAddAccessTokenException : DataException
    {
        public FailedToAddAccessTokenException(string athleteId)
            : base($"Failed to add an access token for athlete: [{athleteId}].")
        {
        }
    }
}
using System.Data;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class FailedToAddNewAthleteException : DataException
    {
        public FailedToAddNewAthleteException(string athleteExternalId)
            : base($"Failed to add a new athlete: [{athleteExternalId}].")
        {
        }
    }
}
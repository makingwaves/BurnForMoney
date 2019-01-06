using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    public class FailedToAddAthleteException : DataException
    {
        public FailedToAddAthleteException(string athleteExternalId)
            : base($"Failed to add a new athlete: [{athleteExternalId}].")
        {
        }
    }
}
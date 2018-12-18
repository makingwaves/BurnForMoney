using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToAddAthleteException : DataException
    {
        public FailedToAddAthleteException(string athleteId)
            : base($"Failed to add athlete: [{athleteId}].")
        {
        }
    }
}
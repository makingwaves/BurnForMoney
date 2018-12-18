using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToDeactivateAthleteException : DataException
    {
        public FailedToDeactivateAthleteException(string athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }
    }
}
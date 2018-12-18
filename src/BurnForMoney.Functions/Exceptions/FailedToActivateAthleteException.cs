using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(string athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }
    }
}
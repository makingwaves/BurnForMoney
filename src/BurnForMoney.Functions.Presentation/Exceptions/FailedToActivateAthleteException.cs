using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(string athleteId)
            : base($"Failed to activate athlete: [{athleteId}].")
        {
        }
    }
}
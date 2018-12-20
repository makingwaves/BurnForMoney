using System.Data;

namespace BurnForMoney.Functions.ReadModel.Exceptions
{
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(string athleteId)
            : base($"Failed to activate athlete: [{athleteId}].")
        {
        }
    }
}
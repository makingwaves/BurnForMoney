using System.Data;

namespace BurnForMoney.Functions.ReadModel.Exceptions
{
    public class FailedToDeactivateAthleteException : DataException
    {
        public FailedToDeactivateAthleteException(string athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }
    }
}
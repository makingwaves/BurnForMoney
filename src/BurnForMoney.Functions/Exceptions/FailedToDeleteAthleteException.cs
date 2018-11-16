using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToDeleteAthleteException : DataException
    {
        public FailedToDeleteAthleteException(string athleteId)
        : base($"Failed to delete athlete: [{athleteId}].")
        {
        }
    }
}
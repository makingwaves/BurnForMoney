using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToAddAthleteException : DataException
    {
        public FailedToAddAthleteException(Guid athleteId)
            : base($"Failed to add a new athlete: [{athleteId}].")
        {
        }
    }
}
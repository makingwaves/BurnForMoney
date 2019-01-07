using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToDeactivateAthleteException : DataException
    {
        public FailedToDeactivateAthleteException(Guid athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }
    }
}
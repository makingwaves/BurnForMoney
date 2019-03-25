using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(Guid athleteId)
            : base($"Failed to activate athlete: [{athleteId}].")
        {}
    }
}
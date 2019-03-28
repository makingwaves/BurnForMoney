using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(string athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }

        protected FailedToActivateAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
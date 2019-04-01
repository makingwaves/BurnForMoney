using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToActivateAthleteException : DataException
    {
        public FailedToActivateAthleteException(Guid athleteId)
            : base($"Failed to activate athlete: [{athleteId}].")
        {
        }

        protected FailedToActivateAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
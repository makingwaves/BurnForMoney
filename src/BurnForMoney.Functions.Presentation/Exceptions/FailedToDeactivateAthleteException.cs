using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToDeactivateAthleteException : DataException
    {
        public FailedToDeactivateAthleteException(Guid athleteId)
            : base($"Failed to deactivate athlete: [{athleteId}].")
        {
        }

        protected FailedToDeactivateAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
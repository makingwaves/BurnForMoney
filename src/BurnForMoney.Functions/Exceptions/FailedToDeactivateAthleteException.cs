using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToDeactivateAthleteException : DataException
    {
        public FailedToDeactivateAthleteException(string athleteId)
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
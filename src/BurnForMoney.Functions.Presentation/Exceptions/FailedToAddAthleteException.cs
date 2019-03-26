using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToAddAthleteException : DataException
    {
        public FailedToAddAthleteException(Guid athleteId)
            : base($"Failed to add a new athlete: [{athleteId}].")
        {
        }

        protected FailedToAddAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
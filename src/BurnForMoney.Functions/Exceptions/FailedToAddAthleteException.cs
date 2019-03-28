using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToAddAthleteException : DataException
    {
        public FailedToAddAthleteException(string athleteId)
            : base($"Failed to add athlete: [{athleteId}].")
        {
        }

        protected FailedToAddAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
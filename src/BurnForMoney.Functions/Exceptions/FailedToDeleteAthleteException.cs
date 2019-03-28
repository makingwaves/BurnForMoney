using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToDeleteAthleteException : DataException
    {
        public FailedToDeleteAthleteException(string athleteId)
            : base($"Failed to delete athlete: [{athleteId}].")
        {
        }

        protected FailedToDeleteAthleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
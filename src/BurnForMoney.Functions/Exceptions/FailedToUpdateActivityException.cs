using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToUpdateActivityException : DataException
    {
        public FailedToUpdateActivityException(string activityId)
            : base($"Failed to update activity: [{activityId}].")
        {
        }

        protected FailedToUpdateActivityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
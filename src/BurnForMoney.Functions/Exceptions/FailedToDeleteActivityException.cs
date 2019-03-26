using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToDeleteActivityException : DataException
    {
        public FailedToDeleteActivityException(string activityId)
            : base($"Failed to delete activity: [{activityId}].")
        {
        }

        protected FailedToDeleteActivityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
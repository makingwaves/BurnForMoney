using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToDeleteActivityException : DataException
    {
        public FailedToDeleteActivityException(Guid activityId)
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
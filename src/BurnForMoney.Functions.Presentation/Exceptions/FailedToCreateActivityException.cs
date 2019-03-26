using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToCreateActivityException : DataException
    {
        public FailedToCreateActivityException(Guid activityId)
            : base($"Failed to add a new activity: [{activityId}].")
        {
        }

        protected FailedToCreateActivityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
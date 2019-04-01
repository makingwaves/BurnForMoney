using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToUpdateActivityException : DataException
    {
        public FailedToUpdateActivityException(Guid activityId)
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
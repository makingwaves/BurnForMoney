using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToUpdateActivityException : DataException
    {
        public FailedToUpdateActivityException(Guid activityId)
            : base($"Failed to update activity: [{activityId}].")
        {
        }
    }
}
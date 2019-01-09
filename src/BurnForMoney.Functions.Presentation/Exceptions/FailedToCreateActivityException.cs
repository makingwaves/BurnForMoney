using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToCreateActivityException : DataException
    {
        public FailedToCreateActivityException(Guid activityId)
            : base($"Failed to add a new activity: [{activityId}].")
        {
        }
    }
}
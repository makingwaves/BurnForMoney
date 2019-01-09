using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class FailedToDeleteActivityException : DataException
    {
        public FailedToDeleteActivityException(Guid activityId)
            : base($"Failed to delete activity: [{activityId}].")
        {
        }
    }
}
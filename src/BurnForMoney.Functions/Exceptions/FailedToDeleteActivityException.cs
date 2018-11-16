using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToDeleteActivityException : DataException
    {
        public FailedToDeleteActivityException(string activityId)
            : base($"Failed to delete activity: [{activityId}].")
        {
        }
    }
}
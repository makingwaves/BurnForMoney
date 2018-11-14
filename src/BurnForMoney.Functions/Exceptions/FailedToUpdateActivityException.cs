using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class FailedToUpdateActivityException : DataException
    {
        public FailedToUpdateActivityException(string activityId)
            : base($"Failed to update activity: [{activityId}].")
        {
        }
    }
}
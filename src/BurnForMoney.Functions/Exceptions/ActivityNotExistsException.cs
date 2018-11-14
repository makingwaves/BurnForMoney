using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class ActivityNotExistsException : DataException
    {
        public ActivityNotExistsException(string activityId, string externalActivityId)
            : base($"Activity with id: [{activityId}], external id: [{externalActivityId}] does not exists.")
        {
        }
    }
}
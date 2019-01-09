using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Events;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class ActivityDeleted : ActivityEvent
    {
        public readonly Guid ActivityId;

        public ActivityDeleted(Guid activityId)
        {
            ActivityId = activityId;
        }
    }
}
using System;

namespace BurnForMoney.Domain.Domain.ActivityMappers
{
    public static class ManualActivityMapper
    {
        public static ActivityCategory MapToActivityCategory(string activityType)
        {
            var isDefined = Enum.IsDefined(typeof(ActivityCategory), activityType);
            if (isDefined)
            {
                return (ActivityCategory)Enum.Parse(typeof(ActivityCategory), activityType);
            }

            return ActivityCategory.Other;
        }
    }
}
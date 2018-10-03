using BurnForMoney.Functions.External.Strava.Api.Model;
using BurnForMoney.Functions.Persistence.DatabaseSchema;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class StravaActivityMapper
    {
        public static ActivityCategory MapToActivityCategory(StravaActivityType activityType)
        {
            switch (activityType)
            {
                case StravaActivityType.Snowboard:
                case StravaActivityType.AlpineSki:
                case StravaActivityType.BackcountrySki:
                case StravaActivityType.IceSkate:
                case StravaActivityType.NordicSki:
                    return ActivityCategory.WinterSports;
                case StravaActivityType.Canoeing:
                case StravaActivityType.Kayaking:
                case StravaActivityType.Kitesurf:
                case StravaActivityType.Rowing:
                case StravaActivityType.StandUpPaddling:
                case StravaActivityType.Surfing:
                case StravaActivityType.Swim:
                case StravaActivityType.Windsurf:
                    return ActivityCategory.WaterSports;
                case StravaActivityType.Crossfit:
                case StravaActivityType.WeightTraining:
                    return ActivityCategory.Gym;
                case StravaActivityType.Ride:
                case StravaActivityType.EBikeRide:
                case StravaActivityType.Handcycle:
                case StravaActivityType.VirtualRide:
                    return ActivityCategory.Ride;
                case StravaActivityType.Run:
                case StravaActivityType.Elliptical:
                case StravaActivityType.VirtualRun:
                    return ActivityCategory.Run;
                case StravaActivityType.Hike:
                case StravaActivityType.RockClimbing:
                case StravaActivityType.Snowshoe:
                    return ActivityCategory.Hike;
                case StravaActivityType.InlineSkate:
                case StravaActivityType.RollerSki:
                case StravaActivityType.StairStepper:
                case StravaActivityType.Wheelchair:
                    return ActivityCategory.Other;
                case StravaActivityType.Walk:
                    return ActivityCategory.Walk;
                case StravaActivityType.Workout:
                case StravaActivityType.Yoga:
                    return ActivityCategory.Fitness;
                default:
                    return ActivityCategory.Other;
            }
        }
    }
}
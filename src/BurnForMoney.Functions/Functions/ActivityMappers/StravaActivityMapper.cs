using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Functions.ActivityMappers
{
    public static class StravaActivityMapper
    {
        public static ActivityCategory MapToActivityCategory(string activityType)
        {
            switch (activityType)
            {
                case "Snowboard":
                case "AlpineSki":
                case "BackcountrySki":
                case "IceSkate":
                case "NordicSki":
                    return ActivityCategory.WinterSports;
                case "Canoeing":
                case "Kayaking":
                case "Kitesurf":
                case "Rowing":
                case "StandUpPaddling":
                case "Surfing":
                case "Swim":
                case "Windsurf":
                    return ActivityCategory.WaterSports;
                case "Crossfit":
                case "WeightTraining":
                    return ActivityCategory.Gym;
                case "Ride":
                case "EBikeRide":
                case "Handcycle":
                case "VirtualRide":
                    return ActivityCategory.Ride;
                case "Run":
                case "Elliptical":
                case "VirtualRun":
                    return ActivityCategory.Run;
                case "Hike":
                case "RockClimbing":
                case "Snowshoe":
                    return ActivityCategory.Hike;
                case "InlineSkate":
                case "RollerSki":
                case "StairStepper":
                case "Wheelchair":
                    return ActivityCategory.Other;
                case "Walk":
                    return ActivityCategory.Walk;
                case "Workout":
                case "Yoga":
                    return ActivityCategory.Fitness;
                default:
                    return ActivityCategory.Other;
            }
        }
    }
}
using BurnForMoney.Functions.Strava.Model;

namespace BurnForMoney.Functions
{
    public class PointsCalculator
    {
        
    }


    public class DefaultPointsCalculatingStrategy : IPointsCalculatingStrategy
    {
        public int Calculate()
        {
            throw new System.NotImplementedException();
        }
    }


    public interface IPointsCalculatingStrategy
    {
        int Calculate();
    }

    public enum ActivityCategory
    {
        Run,
        Ride,
        Walk,
        WinterSports,
        WaterSports,
        TeamSports,
        Gym,
        Hike,
        Fitness,
        Other
    }

    public class StravaActivityMapper
    {
        public static ActivityCategory MapToActivityCategory(ActivityType activityType)
        {
            switch (activityType)
            {
                case ActivityType.Snowboard:
                case ActivityType.AlpineSki:
                case ActivityType.BackcountrySki:
                case ActivityType.IceSkate:
                case ActivityType.NordicSki:
                    return ActivityCategory.WinterSports;
                case ActivityType.Canoeing:
                case ActivityType.Kayaking:
                case ActivityType.Kitesurf:
                case ActivityType.Rowing:
                case ActivityType.StandUpPaddling:
                case ActivityType.Surfing:
                case ActivityType.Swim:
                case ActivityType.Windsurf:
                    return ActivityCategory.WaterSports;
                case ActivityType.Crossfit:
                case ActivityType.WeightTraining:
                    return ActivityCategory.Gym;
                case ActivityType.Ride:
                case ActivityType.EBikeRide:
                case ActivityType.Handcycle:
                case ActivityType.VirtualRide:
                    return ActivityCategory.Ride;
                case ActivityType.Run:
                case ActivityType.Elliptical:
                case ActivityType.VirtualRun:
                    return ActivityCategory.Run;
                case ActivityType.Hike:
                case ActivityType.RockClimbing:
                case ActivityType.Snowshoe:
                    return ActivityCategory.Hike;
                case ActivityType.InlineSkate:
                case ActivityType.RollerSki:
                case ActivityType.StairStepper:
                case ActivityType.Wheelchair:
                    return ActivityCategory.Other;
                case ActivityType.Walk:
                    return ActivityCategory.Walk;
                case ActivityType.Workout:
                case ActivityType.Yoga:
                    return ActivityCategory.Fitness;
                default:
                    return ActivityCategory.Other;
            }
        }
    }
}
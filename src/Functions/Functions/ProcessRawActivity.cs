using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Model;
using BurnForMoney.Functions.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions
{
    public static class ProcessRawActivity
    {
        [FunctionName(FunctionsNames.Q_ProcessRawActivity)]
        public static async Task Q_ProcessRawActivityAsync(ILogger log, ExecutionContext executionContext, 
            [QueueTrigger(QueueNames.PendingRawActivities)] PendingRawActivity rawActivity,
            [Queue(QueueNames.PendingActivities)] CloudQueue pendingActivitiesQueue)
        {
            if (rawActivity.Source != "Strava")
            {
                throw new NotSupportedException($"System: {rawActivity.Source} is not supported.");
            }

            log.LogInformation($"{FunctionsNames.Q_ProcessRawActivity} function processed a request.");

            var activityCategory = StravaActivityMapper.MapToActivityCategory(rawActivity.ActivityType);
            var points = PointsCalculator.Calculate(activityCategory, rawActivity.DistanceInMeters, rawActivity.MovingTimeInMinutes);

            var activity = new PendingActivity
            {
                AthleteId = rawActivity.AthleteId,
                ActivityId = rawActivity.ActivityId,
                StartDate = rawActivity.StartDate,
                ActivityType = rawActivity.ActivityType,
                DistanceInMeters = rawActivity.DistanceInMeters,
                MovingTimeInMinutes = rawActivity.MovingTimeInMinutes,
                Category = activityCategory,
                Points = points,
                Source = "Strava"
            };

            var json = JsonConvert.SerializeObject(activity);
            await pendingActivitiesQueue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }

    internal class PointsCalculator
    {
        public static double Calculate(ActivityCategory category, double distanceInMeters, double timeInMinutes)
        {
            double points;

            switch (category)
            {
                case ActivityCategory.Run:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters, 2); // 1km = 2 points
                    break;
                case ActivityCategory.Ride:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters); // 1km = 1 point
                    break;
                case ActivityCategory.Walk:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters);
                    break;
                case ActivityCategory.WinterSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes); // 10min = 1 point
                    break;
                case ActivityCategory.WaterSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.TeamSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.Gym:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.Hike:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.Fitness:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                default:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
            }

            return Math.Round(points, 2);
        }

        private static double GetPointsForDistanceBasedCategory(double distanceInMeters, double factor = 1)
        {
            return Math.Round(distanceInMeters * factor / 1000, 2);
        }

        private static double GetPointsForTimeBasedCategory(double timeInMinutes, double factor = 1)
        {
            return Math.Round(timeInMinutes * factor / 10, 2);
        }
    }

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
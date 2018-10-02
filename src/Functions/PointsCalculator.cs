using System;
using BurnForMoney.Functions.Model;

namespace BurnForMoney.Functions
{
    public class PointsCalculator
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
}
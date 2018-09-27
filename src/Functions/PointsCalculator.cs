using System;
using BurnForMoney.Functions.Model;

namespace BurnForMoney.Functions
{
    public interface IPointsCalculatingStrategy
    {
        double Calculate(ActivityCategory category, double distanceInMeters, int timeInMinutes);
    }

    public class DefaultPointsCalculatingStrategy: IPointsCalculatingStrategy
    {
        public double Calculate(ActivityCategory category, double distanceInMeters, int timeInMinutes)
        {
            double points;

            switch (category)
            {
                case ActivityCategory.Run:
                    points = distanceInMeters * 2.00 / 1000; // 1km = 2 points
                    break;
                case ActivityCategory.Ride:
                    points = distanceInMeters * 1.00 / 1000; // 1km = 1 point
                    break;
                case ActivityCategory.Walk:
                    points = distanceInMeters * 1.00 / 1000;
                    break;
                case ActivityCategory.WinterSports:
                    points = timeInMinutes * 1.00 / 10; // 10min = 1 point
                    break;
                case ActivityCategory.WaterSports:
                    points = timeInMinutes * 1.00 / 10;
                    break;
                case ActivityCategory.TeamSports:
                    points = timeInMinutes * 1.00 / 10;
                    break;
                case ActivityCategory.Gym:
                    points = timeInMinutes * 1.00 / 10;
                    break;
                case ActivityCategory.Hike:
                    points = distanceInMeters * 1.00 / 1000;
                    break;
                case ActivityCategory.Fitness:
                    points = timeInMinutes * 1.00 / 10;
                    break;
                default:
                    points = timeInMinutes * 1.00 / 10;
                    break;
            }

            return Math.Round(points, 2);
        }
    }
}
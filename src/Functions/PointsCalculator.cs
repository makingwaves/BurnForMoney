using BurnForMoney.Functions.Model;

namespace BurnForMoney.Functions
{
    public class PointsCalculator
    {
        
    }


    public class DefaultPointsCalculatingStrategy : IPointsCalculatingStrategy
    {
        public int Calculate(ActivityCategory cateogry, int distanceInMeters, int timeInMinutes)
        {
            throw new System.NotImplementedException();
        }
    }


    public interface IPointsCalculatingStrategy
    {
        int Calculate(ActivityCategory cateogry, int distanceInMeters, int timeInMinutes);
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class MonthlyResultsView : IHandles<ActivityAdded>, IHandles<ActivityDeleted_V2>, IHandles<ActivityUpdated_V2>
    {
        private readonly string _sqlConnectionString;

        public MonthlyResultsView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public Task HandleAsync(ActivityAdded message)
        {
            return new MonthlyResultsAddStrategy(_sqlConnectionString).CreateOrUpdateResults(
                new MonthlyResultsChangeRequest
                {
                    AthleteId = message.AthleteId,
                    MovingTime = Convert.ToInt32(message.MovingTimeInMinutes),
                    Points = Convert.ToInt32(message.Points),
                    Distance = Convert.ToInt32(message.DistanceInMeters),
                    Category = message.ActivityCategory.ToString(),
                    Month = message.StartDate.Month,
                    Year = message.StartDate.Year
                });
        }

        public Task HandleAsync(ActivityDeleted_V2 message)
        {
            return new MonthlyResultsDeleteStrategy(_sqlConnectionString).CreateOrUpdateResults(
                new MonthlyResultsChangeRequest
                {
                    AthleteId = message.AthleteId,
                    MovingTime = Convert.ToInt32(message.PreviousData.MovingTimeInMinutes) * -1,
                    Points = Convert.ToInt32(message.PreviousData.Points) * -1,
                    Distance = Convert.ToInt32(message.PreviousData.DistanceInMeters) * -1,
                    Category = message.PreviousData.ActivityCategory.ToString(),
                    Month = message.PreviousData.StartDate.Month,
                    Year = message.PreviousData.StartDate.Year
                });
        }

        public Task HandleAsync(ActivityUpdated_V2 message)
        {
            int deltaMovingTime = Convert.ToInt32(message.MovingTimeInMinutes - message.PreviousData.MovingTimeInMinutes);
            int deltaPoints = Convert.ToInt32(message.Points - message.PreviousData.Points);
            int deltaDistance = Convert.ToInt32(message.DistanceInMeters - message.PreviousData.DistanceInMeters);

            return new MonthlyResultsUpdateStrategy(_sqlConnectionString).CreateOrUpdateResults(
                new MonthlyResultsChangeRequest
                {
                    AthleteId = message.AthleteId,
                    MovingTime = deltaMovingTime,
                    Points = deltaPoints,
                    Distance = deltaDistance,
                    Category = message.ActivityCategory.ToString(),
                    Month = message.StartDate.Month,
                    Year = message.StartDate.Year
                });
        }
    }
}
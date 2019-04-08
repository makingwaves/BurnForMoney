using System;
using System.Linq;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsUpdateStrategy : MonthlyResultsChangeStrategy
    {
        public MonthlyResultsUpdateStrategy(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        protected override void UpdateResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request)
        {
            AthleteResult athleteResult = result.AthleteResults.Single(x => x.Id == request.AthleteId);
            UpdateAthleteResultMetrics(request, athleteResult);

            string previousCategory = request.PreviousData.ActivityCategory.ToString();
            if (request.Category != previousCategory)
            {
                AthleteMonthlyResultActivity activity = athleteResult.Activities.Find(x => x.Category == request.Category);
                if (activity == null)
                {
                    activity = new AthleteMonthlyResultActivity {Category = request.Category};
                    athleteResult.Activities.Add(activity);
                }

                UpdateCombinedMetrics(request, activity);
                UpdatePreviousActivity(request, athleteResult, previousCategory);
            }
            else
            {
                AthleteMonthlyResultActivity activity = athleteResult.Activities.Single(x => x.Category == request.Category);
                UpdateActivityMetrics(request, activity);
            }
        }

        private static void UpdateCombinedMetrics(MonthlyResultsChangeRequest request, AthleteMonthlyResultActivity activity)
        {
            activity.Distance = request.PreviousData.DistanceInMeters + request.Distance;
            activity.Points = Convert.ToInt32(request.PreviousData.Points + request.Points);
            activity.Time = request.PreviousData.MovingTimeInMinutes + request.MovingTime;
            activity.NumberOfTrainings += 1;
        }

        private static void UpdatePreviousActivity(MonthlyResultsChangeRequest request, AthleteResult athleteResult, 
            string previousCategory)
        {
            AthleteMonthlyResultActivity previous = athleteResult.Activities.Single(x => x.Category == previousCategory);
            previous.Distance -= request.PreviousData.DistanceInMeters;
            previous.Points -= Convert.ToInt32(request.PreviousData.Points);
            previous.Time -= request.PreviousData.MovingTimeInMinutes;
            previous.NumberOfTrainings -= 1;

            if (previous.NumberOfTrainings <= 0)
            {
                athleteResult.Activities.Remove(previous);
            }
        }
    }
}

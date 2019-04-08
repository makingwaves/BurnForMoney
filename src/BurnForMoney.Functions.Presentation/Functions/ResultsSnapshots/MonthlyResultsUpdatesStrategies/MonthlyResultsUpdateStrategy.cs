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

            AthleteMonthlyResultActivity activity;
            string previousCategory = request.PreviousData.ActivityCategory.ToString();
            if (request.Category != previousCategory)
            {
                activity = athleteResult.Activities.Find(x => x.Category == request.Category);
                if (activity == null)
                {
                    activity = new AthleteMonthlyResultActivity {Category = request.Category, NumberOfTrainings = 1};
                    athleteResult.Activities.Add(activity);
                }

                UpdatePreviousActivity(result, request, athleteResult, previousCategory);
            }
            else
            {
                activity = athleteResult.Activities.Single(x => x.Category == request.Category);
            }
            UpdateActivityMetrics(request, activity);
        }

        private static void UpdatePreviousActivity(AthleteMonthlyResult result, MonthlyResultsChangeRequest request, 
            AthleteResult athleteResult, string previousCategory)
        {
            AthleteMonthlyResultActivity previous = athleteResult.Activities.Single(x => x.Category == previousCategory);
            previous.Distance -= request.PreviousData.DistanceInMeters;
            previous.Points -= Convert.ToInt32(request.PreviousData.Points);
            previous.Time -= request.PreviousData.MovingTimeInMinutes;
            previous.NumberOfTrainings -= 1;

            RemovePreviousActivityMetrics(result, request);

            if (previous.NumberOfTrainings <= 0)
            {
                athleteResult.Activities.Remove(previous);
            }
        }

        private static void RemovePreviousActivityMetrics(AthleteMonthlyResult result, MonthlyResultsChangeRequest request)
        {
            result.Time -= request.PreviousData.MovingTimeInMinutes;
            result.Points -= Convert.ToInt32(request.PreviousData.Points);
            result.Distance -= request.PreviousData.DistanceInMeters;
        }
    }
}

using System.Collections.Generic;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsAddStrategy : MonthlyResultsChangeStrategy
    {
        public MonthlyResultsAddStrategy(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        protected override void UpdateResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request)
        {
            AthleteResult athleteResult = UpdateAthleteResult(result, request);
            UpdateAthleteActivity(request, athleteResult);
        }

        private static void UpdateAthleteActivity(MonthlyResultsChangeRequest request, AthleteResult athleteResult)
        {
            AthleteMonthlyResultActivity activity = athleteResult.Activities.Find(x => x.Category == request.Category);
            if (activity == null)
            {
                activity = new AthleteMonthlyResultActivity {Category = request.Category};
                athleteResult.Activities.Add(activity);
            }

            UpdateActivityMetrics(request, activity);
            activity.NumberOfTrainings += 1;
        }

        private static AthleteResult UpdateAthleteResult(AthleteMonthlyResult result,
            MonthlyResultsChangeRequest request)
        {
            AthleteResult athleteResult = result.AthleteResults.Find(x => x.Id == request.AthleteId);
            if (athleteResult == null)
            {
                athleteResult = CreateInitialResult(request);
                result.AthleteResults.Add(athleteResult);
            }

            UpdateAthleteResultMetrics(request, athleteResult);
            athleteResult.NumberOfTrainings += 1;
            return athleteResult;
        }

        private static AthleteResult CreateInitialResult(MonthlyResultsChangeRequest request)
        {
            return new AthleteResult
            {
                Id = request.AthleteId,
                Activities = new List<AthleteMonthlyResultActivity>()
            };
        }
    }
}
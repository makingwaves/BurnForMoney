using System.Linq;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsDeleteStrategy : MonthlyResultsChangeStrategy
    {
        public MonthlyResultsDeleteStrategy(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        protected override AthleteMonthlyResult GetMonthlyResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request)
        {
            AthleteResult athleteResult = result.AthleteResults.Single(x => x.Id == request.AthleteId);
            int athleteResultIndex = result.AthleteResults.IndexOf(athleteResult);
            UpdateAthleteResultMetrics(request, athleteResult);
            athleteResult.NumberOfTrainings -= 1;

            if (athleteResult.NumberOfTrainings <= 0)
            {
                result.AthleteResults.RemoveAt(athleteResultIndex);
            }
            else
            {
                AthleteMonthlyResultActivity activity = athleteResult.Activities.Single(x => x.Category == request.Category);
                int activityIndex = athleteResult.Activities.IndexOf(activity);
                UpdateActivityMetrics(request, activity);
                activity.NumberOfTrainings -= 1;

                if (activity.NumberOfTrainings <= 0)
                {
                    athleteResult.Activities.RemoveAt(activityIndex);
                }
            }

            return result;
        }
    }
}

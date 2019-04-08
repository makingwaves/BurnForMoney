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

            AthleteMonthlyResultActivity activity = athleteResult.Activities.Single(x => x.Category == request.Category);
            UpdateActivityMetrics(request, activity);
        }
    }
}

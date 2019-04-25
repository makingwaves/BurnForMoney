using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsDeleteStrategy : MonthlyResultsChangeStrategy
    {
        public MonthlyResultsDeleteStrategy(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        protected override async Task UpdateResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request,
            IDbConnection connection, IDbTransaction transaction)
        {
            AthleteResult athleteResult = result.AthleteResults.Single(x => x.Id == request.AthleteId);
            UpdateAthleteResultMetrics(request, athleteResult);
            athleteResult.NumberOfTrainings -= 1;

            if (athleteResult.NumberOfTrainings <= 0)
            {
                result.AthleteResults.Remove(athleteResult);
            }
            else
            {
                string category = request.PreviousData.ActivityCategory.ToString();
                AthleteMonthlyResultActivity activity = athleteResult.Activities.Single(x => x.Category == category);
                UpdateActivityMetrics(request, activity);
                activity.NumberOfTrainings -= 1;

                if (activity.NumberOfTrainings <= 0)
                {
                    athleteResult.Activities.Remove(activity);
                }
            }
        }
    }
}

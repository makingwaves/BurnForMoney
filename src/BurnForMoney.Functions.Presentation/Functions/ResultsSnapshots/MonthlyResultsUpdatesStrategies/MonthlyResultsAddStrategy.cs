using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using Dapper;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsAddStrategy : MonthlyResultsChangeStrategy
    {
        public MonthlyResultsAddStrategy(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        protected override async Task UpdateResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request,
            IDbConnection connection, IDbTransaction transaction)
        {
            AthleteResult athleteResult = await UpdateAthleteResult(result, request, connection, transaction)
                .ConfigureAwait(false);
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

        private static async Task<AthleteResult> UpdateAthleteResult(AthleteMonthlyResult result,
            MonthlyResultsChangeRequest request,
            IDbConnection connection, IDbTransaction transaction)
        {
            AthleteResult athleteResult = result.AthleteResults.Find(x => x.Id == request.AthleteId);
            if (athleteResult == null)
            {
                athleteResult = await CreateInitialResult(request, connection, transaction).ConfigureAwait(false);
                result.AthleteResults.Add(athleteResult);
            }

            UpdateAthleteResultMetrics(request, athleteResult);
            athleteResult.NumberOfTrainings += 1;
            return athleteResult;
        }

        private static async Task<AthleteResult> CreateInitialResult(MonthlyResultsChangeRequest request,
            IDbConnection connection, IDbTransaction transaction)
        {
            dynamic athlete = await connection.QuerySingleAsync(
                "SELECT FirstName, LastName from Athletes where Id = @Id", new
                {
                    Id = request.AthleteId
                }, transaction).ConfigureAwait(false);

            string firstName = athlete.FirstName;
            string lastName = athlete.LastName;

            return new AthleteResult
            {
                Id = request.AthleteId,
                AthleteName = $"{firstName} {lastName}",
                Activities = new List<AthleteMonthlyResultActivity>()
            };
        }
    }
}
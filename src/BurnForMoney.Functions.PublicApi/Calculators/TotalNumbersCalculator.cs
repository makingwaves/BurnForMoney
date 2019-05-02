using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.PublicApi.Calculators
{
    public interface ITotalNumbersCalculator
    {
        Task<TotalNumbers> GetTotalNumbersAsync();
    }

    public class TotalNumbersCalculator : ITotalNumbersCalculator
    {
        private readonly IConnectionProvider<SqlConnection> _connectionProvider;
        private readonly IEmployeesEngagementCalculator _engagementCalculator;

        public TotalNumbersCalculator(IConnectionProvider<SqlConnection> connectionProvider,
            IEmployeesEngagementCalculator engagementCalculator)
        {
            _connectionProvider = connectionProvider;
            _engagementCalculator = engagementCalculator;
        }


        public async Task<TotalNumbers> GetTotalNumbersAsync()
        {
            var today = DateTime.UtcNow;

            using (var conn = _connectionProvider.Create())
            {
                await conn.OpenWithRetryAsync();

                var dto = await conn
                    .QueryAsync<(string date, string json)>("SELECT Date, Results FROM dbo.[MonthlyResultsSnapshots]")
                    .ConfigureAwait(false);

                var results = dto.Select(record =>
                        new
                        {
                            Date = record.date,
                            Results = JsonConvert.DeserializeObject<AthleteMonthlyResult>(record.json)
                        })
                    .OrderBy(month => month.Date, new DateComparer())
                    .ToList();

                var totalDistance = results.Sum(r => r.Results.Distance);
                var totalTime = results.Sum(r => r.Results.Time);
                var totalPoints = results.Sum(r => r.Results.Points);

                var thisMonth = results.SingleOrDefault(r => r.Date.Equals($"{today.Year}/{today.Month}"));

                var result = new TotalNumbers
                {
                    Distance = (int) UnitsConverter.ConvertMetersToKilometers(totalDistance, 0),
                    Time = (int) UnitsConverter.ConvertMinutesToHours(totalTime, 0),
                    Money = PointsToMoneyConverter.Convert(totalPoints),
                    ThisMonth = thisMonth == null ? ThisMonth.NoResults : GetThisMonthStatistics(thisMonth.Results)
                };

                return result;
            }
        }

        private ThisMonth GetThisMonthStatistics(AthleteMonthlyResult thisMonth)
        {
            var uniqueAthletes = thisMonth.AthleteResults.Count;

            var totalPointsThisMonth = thisMonth.Points;
            var mostFrequentActivities = thisMonth
                .AthleteResults
                .SelectMany(r => r.Activities)
                .GroupBy(key => key.Category, el => el, (category, activities) =>
                {
                    activities = activities.ToList();
                    return new
                    {
                        Category = category,
                        NumberOfTrainings = activities.Sum(a => a.NumberOfTrainings),
                        Points = activities.Sum(a => a.Points)
                    };
                })
                .OrderByDescending(o => o.NumberOfTrainings)
                .Take(5);

            return new ThisMonth
            {
                NumberOfTrainings = thisMonth.AthleteResults.Sum(r => r.NumberOfTrainings),
                PercentOfEngagedEmployees = _engagementCalculator.GetPercentOfEngagedEmployees(uniqueAthletes),
                Points = totalPointsThisMonth,
                Money = PointsToMoneyConverter.Convert(totalPointsThisMonth),
                MostFrequentActivities = mostFrequentActivities
            };
        }
    }

    public class PointsToMoneyConverter
    {
        public static int Convert(int points) => points * 100 / 500;
    }
}
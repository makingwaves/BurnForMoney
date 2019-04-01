using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using Newtonsoft.Json;

namespace BurnForMoney.Infrastructure.Persistence.Repositories
{
    public class DashboardReadRepository
    {
        private readonly string _sqlConnectionString;

        public DashboardReadRepository(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task<DashboardTop> GetDashboardTopAsync(int pointsThreshold, decimal payment, int? month = null, int? year = null)
        {
            if (month.HasValue && (month < 1 || month > 12))
            {
                throw new IndexOutOfRangeException(nameof(month));
            }

            if (year.HasValue && year < 2000)
            {
                throw new IndexOutOfRangeException(nameof(year));
            }

                IEnumerable<string> results = await GetJsonResults(month, year);
                return GetDashboard(pointsThreshold, payment, results.ToArray());
        }

        private async Task<IEnumerable<string>> GetJsonResults(int? month, int? year)
        {
            using (SqlConnection connection = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                if (month.HasValue && year.HasValue)
                {
                    string result = await connection.QuerySingleOrDefaultAsync<string>(
                            "SELECT Results FROM dbo.[MonthlyResultsSnapshots] WHERE Date=@Date",
                            new {Date = $"{year}/{month}"}
                        )
                        .ConfigureAwait(false);

                    return string.IsNullOrWhiteSpace(result)
                        ? Enumerable.Empty<string>()
                        : new[] {result};
                }

                return await connection
                    .QueryAsync<string>("SELECT Results FROM dbo.[MonthlyResultsSnapshots]")
                    .ConfigureAwait(false);
            }
        }

        private static DashboardTop GetDashboard(int pointsThreshold, decimal payment, ICollection<string> jsonResults)
        {
            var dashboard = new DashboardTop();
            dashboard.PointsThreshold = pointsThreshold;
            dashboard.Payment = payment;
            if (jsonResults != null && jsonResults.Any())
            {
                var monthlyResults = jsonResults.Select(JsonConvert.DeserializeObject<AthleteMonthlyResult>).ToArray();
                FillDashboardMetrics(dashboard, monthlyResults);
            }

            return dashboard;
        }

        private static void FillDashboardMetrics(DashboardTop dashboard, ICollection<AthleteMonthlyResult> monthlyResults)
        {
            dashboard.TotalDistance = monthlyResults.Sum(res => res.Distance);
            dashboard.TotalPoints = monthlyResults.Sum(res => res.Points);
            dashboard.TotalTime = monthlyResults.Sum(res => res.Time);
            // ReSharper disable once PossibleLossOfFraction - intended
            dashboard.TotalMoney = dashboard.TotalPoints / dashboard.PointsThreshold * dashboard.Payment;
            dashboard.CurrentPoints = dashboard.TotalPoints % dashboard.PointsThreshold;
        }
    }
}
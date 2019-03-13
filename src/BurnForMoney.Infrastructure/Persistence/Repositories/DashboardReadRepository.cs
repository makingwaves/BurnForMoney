using System;
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

        public async Task<DashboardTop> GetDashboardTopAsync(int? month = null, int? year = null)
        {
            if (month.HasValue && (month < 1 || month > 12))
            {
                throw new IndexOutOfRangeException(nameof(month));
            }
            if (year.HasValue && year < 2000)
            {
                throw new IndexOutOfRangeException(nameof(year));
            }

            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var json = await conn.QuerySingleAsync<string>("SELECT Results FROM dbo.[MonthlyResultsSnapshots] WHERE Date=@Date", new {
                    Date = $"{year ?? DateTime.UtcNow.Year}/{month ?? DateTime.UtcNow.Month}"
                }).ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<DashboardTop>(json);

                return result;
            }
        }
    }
}
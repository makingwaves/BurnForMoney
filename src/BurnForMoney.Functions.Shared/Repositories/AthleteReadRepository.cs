using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;

namespace BurnForMoney.Functions.Shared.Repositories
{
    public class AthleteReadRepository : IReadFacade<AthleteRow>
    {
        private readonly string _connectionString;

        public AthleteReadRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AthleteWithStravaIdExistsAsync(string id)
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var exists = await conn.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM dbo.Athletes WHERE ExternalId=@ExternalId", new
                {
                    ExternalId = id
                });
                return exists;
            }
        }

        public async Task<AthleteRow> GetAthleteByStravaIdAsync(string id)
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id FROM dbo.Athletes WHERE ExternalId=@ExternalId", new
                {
                    ExternalId = id
                });
                return athlete;
            }
        }
    }
}
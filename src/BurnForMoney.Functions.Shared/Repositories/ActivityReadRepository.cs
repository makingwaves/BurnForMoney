using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;

namespace BurnForMoney.Functions.Shared.Repositories
{
    public class ActivityReadRepository : IReadFacade<ActivityRow>
    {
        private readonly string _sqlConnectionString;

        public ActivityReadRepository(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task<ActivityRow> GetByExternalIdAsync(string externalId)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = await conn.QuerySingleAsync<ActivityRow>(
                    "SELECT * FROM dbo.Activites WHERE ExternalId=@ExternalId", new
                    {
                        ExternalId = externalId
                    });
                return activity;
            }
        }
    }
}
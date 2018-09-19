using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Strava.Repository
{
    public class ActivityRepository : IRepository
    {
        private readonly string _connectionString;
        private readonly TraceWriter _log;

        public ActivityRepository(string connectionString, TraceWriter log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public async Task BootstrapAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("CREATE TABLE dbo.[Strava.Activities] ([AthleteId][int] NOT NULL, [ActivityId][int] NOT NULL, [ActivityTime][datetime2], [ActivityType][nvarchar](50), [Distance][int], [MovingTime][int], FOREIGN KEY (AthleteId) REFERENCES dbo.[Strava.Athletes](AthleteId))")
                    .ConfigureAwait(false);

                _log.Info("dbo.[Strava.Activities] table created.");
            }
        }
    }
}
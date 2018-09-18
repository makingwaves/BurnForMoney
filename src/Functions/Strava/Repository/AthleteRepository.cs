using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Strava.Model;
using Dapper;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Strava.Repository
{
    public class AthleteRepository
    {
        private readonly string _connectionString;
        private readonly TraceWriter _log;

        public AthleteRepository(string connectionString, TraceWriter log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        public async Task BootstrapAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("CREATE TABLE dbo.[Strava.Athletes] ([AthleteId][int] NOT NULL, [FirstName][nvarchar](50), [LastName][nvarchar](50), [AccessToken][nvarchar](100) NOT NULL, [Active][bit] NOT NULL, PRIMARY KEY (AthleteId))")
                    .ConfigureAwait(false);

                _log.Info("dbo.[Strava.Athletes] table created.");
            }
        }

        public async Task AddAsync(Athlete athlete, string accessToken)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("INSERT INTO dbo.[Strava.Athletes] (AthleteId, FirstName, LastName, AccessToken, Active) VALUES ( @Id, @FirstName, @LastName, @Token, @Active);",
                        new
                        {
                            Id = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            Token = accessToken,
                            Active = true
                        })
                    .ConfigureAwait(false);

                _log.Info($"Athlete {athlete.Firstname} {athlete.Lastname} has been added successfully");
            }
        }
    }
}
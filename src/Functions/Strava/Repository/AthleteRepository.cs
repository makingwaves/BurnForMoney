using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Strava.Model;
using Dapper;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Repository
{
    public class AthleteRepository : IRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _log;
        private readonly string _accessTokensEncryptionKey;

        public AthleteRepository(string connectionString, ILogger log, string accessTokensEncryptionKey)
        {
            _connectionString = connectionString;
            _log = log;
            _accessTokensEncryptionKey = accessTokensEncryptionKey;
        }

        public async Task BootstrapAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("CREATE TABLE dbo.[Strava.Athletes] ([AthleteId][int] NOT NULL, [FirstName][nvarchar](50), [LastName][nvarchar](50), [AccessToken][nvarchar](100) NOT NULL, [Active][bit] NOT NULL, PRIMARY KEY (AthleteId))")
                    .ConfigureAwait(false);
                
                await conn.ExecuteAsync(StoredProcedures.StoredProcedures.Strava_Athlete_Upsert)
                    .ConfigureAwait(false);

                _log.LogInformation("dbo.[Strava.Athletes] table created.");
            }
        }

        public async Task<List<string>> GetAllActiveAccessTokensAsync()
        {
            var tokens = Enumerable.Empty<string>();
            using (var conn = new SqlConnection(_connectionString))
            {
                tokens = await conn.QueryAsync<string>("SELECT AccessToken FROM dbo.[Strava.Athletes] where Active = 1")
                    .ConfigureAwait(false);
            }

            return tokens.Select(DecryptAccessToken).ToList();
        }

        public async Task UpsertAsync(Athlete athlete, string accessToken)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(nameof(StoredProcedures.StoredProcedures.Strava_Athlete_Upsert),
                        new
                        {
                            AthleteId = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            AccessToken = EncryptAccessToken(accessToken),
                            Active = true
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                _log.LogInformation($"Athlete: {athlete.Firstname} {athlete.Lastname} has been saved successfully");
            }
        }

        private string DecryptAccessToken(string encryptedAccessToken)
        {
            var decryptedToken = Cryptography.DecryptString(encryptedAccessToken, _accessTokensEncryptionKey);
            _log.LogInformation("Access token has been decrypted.");

            return decryptedToken;
        }

        private string EncryptAccessToken(string accessToken)
        {
            var encryptedToken = Cryptography.EncryptString(accessToken, _accessTokensEncryptionKey);
            _log.LogInformation("Access token has been encrypted.");

            return encryptedToken;
        }
    }
}
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Strava.Model;
using Dapper;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Services
{
    public class AthleteService
    {
        private readonly string _connectionString;
        private readonly ILogger _log;
        private readonly IAccessTokensEncryptionService _encryptionService;

        public AthleteService(string connectionString, ILogger log, IAccessTokensEncryptionService encryptionService)
        {
            _connectionString = connectionString;
            _log = log;
            _encryptionService = encryptionService;
        }

        public async Task<List<string>> GetAllActiveAccessTokensAsync()
        {
            var tokens = Enumerable.Empty<string>();
            using (var conn = new SqlConnection(_connectionString))
            {
                tokens = await conn.QueryAsync<string>("SELECT AccessToken FROM dbo.[Strava.Athletes] where Active = 1").ConfigureAwait(false);
            }

            return tokens.Select(_encryptionService.DecryptAccessToken).ToList();
        }

        public async Task UpsertAsync(Athlete athlete, string accessToken)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("Strava_Athlete_Upsert",
                        new
                        {
                            AthleteId = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            AccessToken = _encryptionService.EncryptAccessToken(accessToken),
                            Active = true
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                _log.LogInformation($"Athlete: {athlete.Firstname} {athlete.Lastname} has been saved successfully");
            }
        }

        
    }
}
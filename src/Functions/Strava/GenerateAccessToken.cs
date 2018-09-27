using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Auth;
using BurnForMoney.Functions.Strava.Services;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class GenerateAccessToken
    {
        private static ConfigurationRoot _configuration;
        private static string _accessTokenEncryptionKey;
        private static ILogger _log;

        [FunctionName("GenerateAccessToken")]
        public static async Task RunGenerateAccessToken([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, ILogger log,
            ExecutionContext context)
        {
            _log = log;
            _log.LogInformation("GenerateAccessToken function processed a request.");

            await LoadSettingsAsync(context).ConfigureAwait(false);

            _log.LogInformation($"Requesting for access token using clientId: {_configuration.Strava.ClientId}.");
            var response = RequestForAccessToken(_configuration.Strava.ClientId, _configuration.Strava.ClientSecret, myQueueItem);
           
            await UpsertAsync(response.Athlete, response.AccessToken).ConfigureAwait(false);
        }

        public static async Task UpsertAsync(Athlete athlete, string accessToken)
        {
            var encryptionService = new AccessTokensEncryptionService(_log, _accessTokenEncryptionKey);
            using (var conn = new SqlConnection(_configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.ExecuteAsync("Strava_Athlete_Upsert",
                        new
                        {
                            AthleteId = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            AccessToken = encryptionService.EncryptAccessToken(accessToken),
                            Active = true
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                _log.LogInformation($"Athlete: {athlete.Firstname} {athlete.Lastname} has been saved successfully");
            }
        }

        private static async Task LoadSettingsAsync(ExecutionContext context)
        {
            if (_configuration != null)
            {
                return;
            }

            _configuration = ApplicationConfiguration.GetSettings(context);

            if (string.IsNullOrEmpty(_accessTokenEncryptionKey))
            {
                var keyVaultClient = KeyVaultClientFactory.Create();
                var secret = await keyVaultClient.GetSecretAsync(_configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                    .ConfigureAwait(false);
                _accessTokenEncryptionKey = secret.Value;
            }
        }

        private static TokenExchangeResponse RequestForAccessToken(int clientId, string clientSecret, string code)
        {
            var stravaService = new StravaService();
            return stravaService.ExchangeToken(clientId, clientSecret, code);
        }
    }
}

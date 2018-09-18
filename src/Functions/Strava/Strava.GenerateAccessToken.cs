using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Auth;
using BurnForMoney.Functions.Strava.Repository;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Strava
{
    public static class GenerateAccessToken
    {
        private static ConfigurationRoot _configuration;
        private static string _accessTokenEncryptionKey;
        private static TraceWriter _log;

        [FunctionName("GenerateAccessToken")]
        public static async Task RunGenerateAccessToken([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, TraceWriter log,
            ExecutionContext context)
        {
            _log = log;
            _log.Info("GenerateAccessToken function processed a request.");

            LoadSettings(context);

            var response = RequestForAccessToken(_configuration.Strava.ClientId, _configuration.Strava.ClientSecret, myQueueItem);
            var encryptedAccessToken = await EncryptAccessTokenAsync(response.AccessToken, _configuration.ConnectionStrings.KeyVaultConnectionString).ConfigureAwait(false);
            response.AccessToken = encryptedAccessToken;

            var repository = new AthleteRepository(_configuration.ConnectionStrings.SqlDbConnectionString, _log);
            await repository.UpsertAsync(response.Athlete, response.AccessToken).ConfigureAwait(false);
        }

        private static void LoadSettings(ExecutionContext context)
        {
            if (_configuration != null)
            {
                return;
            }

            _configuration = new ApplicationConfiguration().GetSettings(context);
        }

        private static TokenExchangeResponse RequestForAccessToken(int clientId, string clientSecret, string code)
        {
            var stravaService = new StravaService();
            var response = stravaService.ExchangeToken(clientId, clientSecret, code);

            return TokenExchangeResponse.FromJson(response.Content);
        }

        private static async Task<string> EncryptAccessTokenAsync(string accessToken, string keyVaultConnectionString)
        {
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedToken = Cryptography.EncryptString(accessToken, encryptionKey);
            _log.Info("Access token has been encrypted.");

            return encryptedToken;
        }

        private static async Task<string> GetEncryptionKeyAsync(string keyVaultConnectionString)
        {
            if (string.IsNullOrEmpty(_accessTokenEncryptionKey))
            {
                var keyVaultClient = KeyVaultClientFactory.Create();
                var secret = await keyVaultClient.GetSecretAsync(keyVaultConnectionString + "secrets/" + KeyVaultSecretNames.StravaTokensEncryptionKey)
                    .ConfigureAwait(false);
                _accessTokenEncryptionKey = secret.Value;
            }

            return _accessTokenEncryptionKey;
        }
    }
}

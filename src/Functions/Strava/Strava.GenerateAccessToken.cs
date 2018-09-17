using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Auth;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Api;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Strava
{
    public static class GenerateAccessToken
    {
        private const string KeyVaultSecretNameOfEncryptionKey = "stravaAccessTokensEncryptionKey";
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();
        private static string _accessTokenEncryptionKey;
        private static TraceWriter _log;

        [FunctionName("GenerateAccessToken")]
        public static async Task RunGenerateAccessToken([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, TraceWriter log,
            ExecutionContext context)
        {
            _log = log;
            _log.Info("GenerateAccessToken function processed a request.");

            var settings = Configuration.GetSettings(context);
            if (!settings.IsValid())
            {
                throw new Exception("Cannot read configuration file.");
            }

            var accessToken = RequestForAccessToken(settings.Strava.ClientId, settings.Strava.ClientSecret, myQueueItem);
            var encryptedAccessToken = await EncryptAccessTokenAsync(accessToken, settings.ConnectionStrings.KeyVaultConnectionString).ConfigureAwait(false);

            await InsertAccessTokenToSqlDatabaseAsync(settings.ConnectionStrings.SqlDbConnectionString, encryptedAccessToken)
                .ConfigureAwait(false);
        }

        private static async Task<EncryptedAccessToken> EncryptAccessTokenAsync(AccessToken accessToken, string keyVaultConnectionString)
        {
            var encryptionKey = await GetEncryptionKeyAsync(keyVaultConnectionString).ConfigureAwait(false);
            var encryptedToken = Cryptography.EncryptString(accessToken.Token, encryptionKey);
            _log.Info("Access token has been encrypted.");

            return new EncryptedAccessToken(encryptedToken);
        }

        private static async Task<string> GetEncryptionKeyAsync(string keyVaultConnectionString)
        {
            if (string.IsNullOrEmpty(_accessTokenEncryptionKey))
            {
                var keyVaultClient = KeyVaultClientFactory.Create();
                var secret = await keyVaultClient.GetSecretAsync(keyVaultConnectionString + "secrets/" + KeyVaultSecretNameOfEncryptionKey)
                    .ConfigureAwait(false);
                _accessTokenEncryptionKey = secret.Value;
            }

            return _accessTokenEncryptionKey;
        }

        private static AccessToken RequestForAccessToken(int clientId, string clientSecret, string code)
        {
            var stravaService = new StravaService();
            var response = stravaService.ExchangeToken(clientId, clientSecret, code);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }

            return new AccessToken(TokenExchangeResponse.FromJson(response.Content).AccessToken);
        }

        private static async Task InsertAccessTokenToSqlDatabaseAsync(string connectionString, EncryptedAccessToken accessToken)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.ExecuteAsync("INSERT INTO dbo.[Strava.AccessTokens] (AccessToken, Active) VALUES (@Token, 1);",
                        accessToken)
                    .ConfigureAwait(false);

                _log.Info("Access token has been saved successfully");
            }
        }
    }
}

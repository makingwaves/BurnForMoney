using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Auth;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using RestSharp;

namespace BurnForMoney.Functions.Strava
{
    public static class GenerateAccessToken
    {
        private const string StravaBaseUrl = "https://www.strava.com";
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();
        private static KeyVaultClient keyVaultClient;
        private const string KeyVaultSecretNameOfEncryptionKey = "stravaAccessTokensEncryptionKey";

        [FunctionName("GenerateAccessToken")]
        public static async Task RunGenerateAccessToken([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, TraceWriter log,
            ExecutionContext context)
        {
            log.Info("GenerateAccessToken function processed a request.");

            var settings = Configuration.GetSettings(context);
            if (!settings.IsValid())
            {
                throw new Exception("Cannot read configuration file.");
            }

            var a = await EncryptAccessTokenAsync(new AccessToken("ac"), settings.ConnectionStrings.KeyVaultConnectionString).ConfigureAwait(false);

            var accessToken = GetAccessToken(settings.Strava.ClientId, settings.Strava.ClientSecret, myQueueItem);

            await InsertAccessTokenToSqlDatabaseAsync(settings.ConnectionStrings.SqlDbConnectionString, accessToken, log)
                .ConfigureAwait(false);
        }

        private static async Task<EncryptedAccessToken> EncryptAccessTokenAsync(AccessToken accessToken, string keyVaultConnectionString)
        {
            if (keyVaultClient == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            }

            //var secret = await keyVaultClient.GetSecretAsync(keyVaultConnectionString + "secrets/" + KeyVaultSecretNameOfEncryptionKey)
            //    .ConfigureAwait(false);

            return new EncryptedAccessToken(accessToken.Token);
        }

        private static AccessToken GetAccessToken(int clientId, string clientSecret, string code)
        {
            var client = new RestClient(StravaBaseUrl);
            var request = new RestRequest("/oauth/token", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var payLoad = new TokenExchangeRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code
            };
            request.AddParameter("application/json", payLoad.ToJson(), ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }

            return new AccessToken(TokenExchangeResponse.FromJson(response.Content).AccessToken);
        }

        private static async Task InsertAccessTokenToSqlDatabaseAsync(string connectionString, AccessToken accessToken, TraceWriter log)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.ExecuteAsync("INSERT INTO dbo.[Strava.AccessTokens] (AccessToken, Active) VALUES (@Token, 1);",
                        accessToken)
                    .ConfigureAwait(false);

                log.Info("Added token to database.");
            }
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Auth;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using RestSharp;

namespace BurnForMoney.Functions
{
    public static class GenerateAccessToken
    {
        private const string StravaBaseUrl = "https://www.strava.com";
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();

        [FunctionName("GenerateAccessToken")]
        public static async Task Run([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, TraceWriter log,
            ExecutionContext context)
        {
            log.Info("GenerateAccessToken function processed a request.");

            var settings = Configuration.GetSettings(context);
            if (!settings.IsValid())
            {
                throw new Exception("Cannot read configuration file.");
            }

            var accessToken = GetAccessToken(settings.Strava.ClientId, settings.Strava.ClientSecret, myQueueItem);

            await InsertAccessTokenToSqlDatabaseAsync(settings.SqlDbConnectionString, accessToken, log)
                .ConfigureAwait(false);
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
                throw new Exception(response.ErrorMessage);
            }

            return new AccessToken
            {
                Token = TokenExchangeResponse.FromJson(response.Content).AccessToken,
                Active = true
            };
        }

        private static async Task InsertAccessTokenToSqlDatabaseAsync(string connectionString, AccessToken accessToken, TraceWriter log)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.ExecuteAsync("INSERT INTO dbo.[Strava.AccessTokens] (AccessToken, Active) VALUES (@Token, @Active);",
                        accessToken)
                    .ConfigureAwait(false);

                log.Info("Added token to database.");
            }
        }
    }
}

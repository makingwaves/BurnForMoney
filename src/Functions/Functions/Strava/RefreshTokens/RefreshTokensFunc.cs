using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.RefreshTokens
{
    public static class RefreshTokensFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.T_RefreshAccessTokens)]
        public static async Task T_RefreshAccessTokens([TimerTrigger("0 50 * * * *")] TimerInfo timer, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.RefreshStravaToken)] CloudQueue refreshTokensQueue)
        {
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            IEnumerable<(int AthleteId, string EncryptedRefreshToken)> expiringTokens;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                expiringTokens = await conn.QueryAsync<(int, string)>("SELECT AthleteId, RefreshToken as EncryptedRefreshToken FROM dbo.[Strava.AccessTokens] WHERE ExpiresAt < @DateTo",
                    new
                    {
                        DateTo = DateTime.UtcNow.AddHours(1)
                    });
            }

            var tasks = new List<Task>();
            foreach (var (athleteId, encryptedRefreshToken) in expiringTokens)
            {
                var request = new TokenRefreshRequest
                {
                    AthleteId = athleteId,
                    EncryptedRefreshToken = encryptedRefreshToken
                };
                var json = JsonConvert.SerializeObject(request);
                tasks.Add(refreshTokensQueue.AddMessageAsync(new CloudQueueMessage(json)));
            }

            await Task.WhenAll(tasks);
        }

        [FunctionName(FunctionsNames.Q_RefreshAccessTokens)]
        public static async Task Q_RefreshAccessTokens([QueueTrigger(QueueNames.RefreshStravaToken)] TokenRefreshRequest request, ILogger log, ExecutionContext executionContext)
        {
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            var refreshToken = AccessTokensEncryptionService.Decrypt(request.EncryptedRefreshToken, configuration.Strava.AccessTokensEncryptionKey);

            var response = StravaService.RefreshToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                refreshToken);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("UPDATE dbo.[Strava.AccessTokens] SET AccessToken=@AccessToken, RefreshToken=@RefreshToken, ExpiresAt=@ExpiresAt WHERE AthleteId=@AthleteId", new
                {
                    AccessToken = AccessTokensEncryptionService.Encrypt(response.AccessToken, configuration.Strava.AccessTokensEncryptionKey),
                    RefreshToken = AccessTokensEncryptionService.Encrypt(response.RefreshToken, configuration.Strava.AccessTokensEncryptionKey),
                    response.ExpiresAt,
                    request.AthleteId
                });
                if (affectedRows != 1)
                {
                    throw new Exception("Failed to refresh access tokens.");
                }
            }
        }
    }
}
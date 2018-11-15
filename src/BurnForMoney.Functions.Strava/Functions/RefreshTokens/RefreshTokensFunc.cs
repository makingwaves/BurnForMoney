using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.Functions.RefreshTokens.Dto;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.RefreshTokens
{
    public static class RefreshTokensFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.T_RefreshAccessTokens)]
        public static async Task T_RefreshAccessTokens([TimerTrigger("0 50 * * * *")] TimerInfo timer, ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.RefreshStravaToken)] CloudQueue refreshTokensQueue)
        {
            log.LogFunctionStart(FunctionsNames.T_RefreshAccessTokens);
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            IEnumerable<(string AthleteId, string EncryptedRefreshToken)> expiringTokens;
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                expiringTokens = await conn.QueryAsync<(string, string)>(@"SELECT AthleteId, RefreshToken as EncryptedRefreshToken
FROM dbo.[Strava.AccessTokens] AS Tokens
INNER JOIN dbo.Athletes AS Athletes ON (Athletes.Id = Tokens.AthleteId)
WHERE Tokens.ExpiresAt < @DateTo AND Athletes.Active=1",
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
            log.LogFunctionEnd(FunctionsNames.T_RefreshAccessTokens);
        }

        [FunctionName(FunctionsNames.Q_RefreshAccessTokens)]
        public static async Task Q_RefreshAccessTokens([QueueTrigger(QueueNames.RefreshStravaToken)] TokenRefreshRequest request, ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart(FunctionsNames.Q_RefreshAccessTokens);
            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            var refreshToken = AccessTokensEncryptionService.Decrypt(request.EncryptedRefreshToken, configuration.Strava.AccessTokensEncryptionKey);

            var response = StravaService.RefreshToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                refreshToken);

            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("UPDATE dbo.[Strava.AccessTokens] SET AccessToken=@AccessToken, RefreshToken=@RefreshToken, ExpiresAt=@ExpiresAt, IsValid=1 WHERE AthleteId=@AthleteId", new
                {
                    AccessToken = AccessTokensEncryptionService.Encrypt(response.AccessToken, configuration.Strava.AccessTokensEncryptionKey),
                    RefreshToken = AccessTokensEncryptionService.Encrypt(response.RefreshToken, configuration.Strava.AccessTokensEncryptionKey),
                    response.ExpiresAt,
                    request.AthleteId
                });
                if (affectedRows != 1)
                {
                    throw new FailedToRefreshAccessTokenException(request.AthleteId);
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_RefreshAccessTokens);
        }
    }
}
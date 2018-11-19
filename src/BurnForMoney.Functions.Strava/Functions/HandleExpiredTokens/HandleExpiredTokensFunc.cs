using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Strava.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions.HandleExpiredTokens
{
    public static class HandleExpiredTokensFunc
    {
        [FunctionName(FunctionsNames.Q_DeactivateExpiredAccessTokens)]
        public static async Task Q_DeactivateExpiredAccessTokens(ILogger log, 
            [QueueTrigger(QueueNames.UnauthorizedAccessTokens)] string encryptedAccessToken)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeactivateExpiredAccessTokens);

            var configuration = ApplicationConfiguration.GetSettings();
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"IF (Select ExpiresAt from dbo.[Strava.AccessTokens] WHERE AccessToken=@AccessToken) < @DateNow
                                            UPDATE dbo.[Strava.AccessTokens] SET IsValid=0 WHERE AccessToken=@AccessToken",
                    new
                    {
                        DateNow = DateTime.UtcNow,
                        AccessToken = encryptedAccessToken
                    });
                if (affectedRows == 1)
                {
                    log.LogInformation(FunctionsNames.Q_DeactivateExpiredAccessTokens, "Deactivated inactive access token.");
                }
            }
            log.LogFunctionEnd(FunctionsNames.Q_DeactivateExpiredAccessTokens);
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class HandleExpiredTokensFunc
    {
        [FunctionName(FunctionsNames.Strava_Q_DeactivateExpiredAccessTokens)]
        public static async Task Q_DeactivateExpiredAccessTokens(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(QueueNames.UnauthorizedAccessTokens)] string encryptedAccessToken)
        {
            log.LogInformation($"{FunctionsNames.Strava_Q_DeactivateExpiredAccessTokens} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(@"IF (Select ExpiresAt from dbo.[Strava.AccessTokens] WHERE AccessToken=@AccessToken) < @DateNow
                                            UPDATE dbo.[Strava.AccessTokens] SET IsValid=0 WHERE AccessToken=@AccessToken",
                    new
                    {
                        DateNow = DateTime.UtcNow,
                        AccessToken = encryptedAccessToken
                    });
                if (affectedRows == 1)
                {
                    log.LogInformation("Deactivated inactive access token.");
                }
            }
        }
    }
}
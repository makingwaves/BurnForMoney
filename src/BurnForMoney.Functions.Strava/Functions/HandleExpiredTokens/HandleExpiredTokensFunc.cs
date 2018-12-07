using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions.HandleExpiredTokens
{
    public static class HandleExpiredTokensFunc
    {
        [FunctionName(FunctionsNames.Q_DeactivateExpiredAccessTokens)]
        public static async Task Q_DeactivateExpiredAccessTokens(ILogger log, 
            [QueueTrigger(QueueNames.UnauthorizedAthletes)] Guid athleteId,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeactivateExpiredAccessTokens);

            await AccessTokensStore.DeactivateAccessTokenOfAsync(athleteId, configuration.Strava.AccessTokensKeyVaultUrl);
            log.LogInformation(nameof(FunctionsNames.Q_DeactivateExpiredAccessTokens), $"Disabled access token for athlete: {athleteId}.");

            log.LogFunctionEnd(FunctionsNames.Q_DeactivateExpiredAccessTokens);
        }
    }
}
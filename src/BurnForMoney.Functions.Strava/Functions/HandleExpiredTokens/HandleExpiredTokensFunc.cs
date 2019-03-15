using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions.HandleExpiredTokens
{
    public static class HandleExpiredTokensFunc
    {
        [FunctionName(FunctionsNames.Q_DeactivateExpiredAccessTokens)]
        public static async Task Q_DeactivateExpiredAccessTokens(ILogger log, 
            [QueueTrigger(StravaQueueNames.UnauthorizedAthletes)] string athleteId,
            [Configuration] ConfigurationRoot configuration)
        {
            await AccessTokensStore.DeactivateAccessTokenOfAsync(Guid.Parse(athleteId), configuration.Strava.AccessTokensKeyVaultUrl);
            log.LogInformation(nameof(FunctionsNames.Q_DeactivateExpiredAccessTokens), $"Disabled access token for athlete: {athleteId}.");
        }
    }
}
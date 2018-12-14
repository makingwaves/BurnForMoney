using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BurnForMoney.Functions.Strava.Functions.RefreshTokens
{
    public static class RefreshTokensFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.T_RefreshAccessTokens)]
        public static async Task T_RefreshAccessTokens([TimerTrigger("0 50 * * * *")] TimerInfo timer, ILogger log,
            [Queue(StravaQueueNames.RefreshStravaToken)] CloudQueue refreshTokensQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.T_RefreshAccessTokens);

            var secrets = await AccessTokensStore.GetAllSecretsAsync(configuration.Strava.AccessTokensKeyVaultUrl);

            var expiringSecretsMetadata = secrets.Where(secret =>
                secret.Id.EndsWith(AccessTokensSecretNameConvention.AccessTokenSufix) &&
                secret.Attributes.Enabled.HasValue && secret.Attributes.Enabled.Value &&
                secret.Attributes.Expires.HasValue && secret.Attributes.Expires <= DateTime.UtcNow.AddHours(1));

            var athleteIdsWithExpiringTokens =
                expiringSecretsMetadata.Select(metadata => metadata.Tags[AccessTokensTag.AthleteId]);

            var tasks = new List<Task>();
            foreach (var athleteId in athleteIdsWithExpiringTokens)
            {
                tasks.Add(refreshTokensQueue.AddMessageAsync(new CloudQueueMessage(Guid.Parse(athleteId).ToString())));
            }

            await Task.WhenAll(tasks);
            log.LogFunctionEnd(FunctionsNames.T_RefreshAccessTokens);
        }


        [FunctionName(FunctionsNames.Q_RefreshAccessTokens)]
        public static async Task Q_RefreshAccessTokens([QueueTrigger(StravaQueueNames.RefreshStravaToken)] Guid athleteId, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_RefreshAccessTokens);

            var refreshTokenSecret =
                await AccessTokensStore.GetRefreshTokenForAsync(athleteId, configuration.Strava.AccessTokensKeyVaultUrl);
            var refreshToken = refreshTokenSecret.Value;

            var response = StravaService.RefreshToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret,
                refreshToken);

            await AccessTokensStore.AddAsync(athleteId, response.AccessToken, response.RefreshToken, response.ExpiresAt,
                configuration.Strava.AccessTokensKeyVaultUrl);
            log.LogInformation(nameof(FunctionsNames.Q_RefreshAccessTokens), $"Updated tokens for athlete with id: {configuration.Strava.ClientId}.");

            log.LogFunctionEnd(FunctionsNames.Q_RefreshAccessTokens);
        }
    }
}
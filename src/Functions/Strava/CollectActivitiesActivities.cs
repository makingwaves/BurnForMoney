using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesActivities
    {
        [FunctionName(FunctionsNames.A_GetAccessTokens)]
        public static async Task<string[]> GetAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_GetAccessTokens} function processed a request. Instance id: `{activityContext.InstanceId}`");

            var configuration = ApplicationConfiguration.GetSettings(context);
            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(
                configuration.ConnectionStrings.KeyVaultConnectionString,
                KeyVaultSecretNames.StravaTokensEncryptionKey);
            var accessTokenEncryptionKey = secret.Value;

            var encryptionService = new AccessTokensEncryptionService(log, accessTokenEncryptionKey);
            var athletesService = new AthleteService(configuration.ConnectionStrings.SqlDbConnectionString, log, encryptionService);
            var accessTokens = await athletesService.GetAllActiveAccessTokensAsync();
            log.LogInformation($"Received information about {accessTokens.Count} active access tokens.");

            return accessTokens.ToArray();
        }

        [FunctionName(FunctionsNames.A_SaveSingleUserActivities)]
        public static async Task SaveSingleUserActivitiesAsync([ActivityTrigger]string accessToken, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_SaveSingleUserActivities} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(context);
            var activityService = new StravaActivityService(configuration.ConnectionStrings.SqlDbConnectionString, log);
            var stravaService = new StravaService();

            var updateHistoryState = await activityService.GetState();
            var lastUpdate = updateHistoryState.LastUpdate ?? DateTime.UtcNow.AddMonths(-3);
            var activities = stravaService.GetActivitiesFrom(accessToken, lastUpdate);
            foreach (var activity in activities)
            {
                await activityService.InsertAsync(activity);
            }

            await activityService.SaveState();
        }
    }
}

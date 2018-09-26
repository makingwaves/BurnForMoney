using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesActivities
    {
        [FunctionName("A_GetAccessTokens")]
        public static async Task<string[]> GetAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"GetAccessTokens function processed a request. Instance id: `{activityContext.InstanceId}`");

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

        [FunctionName("A_SaveSingleUserActivities")]
        public static async Task SaveSingleUserActivitiesAsync([ActivityTrigger]string accessToken, ILogger log, ExecutionContext context)
        {
            log.LogInformation("CollectSingleUserActivities function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(context);
            var activityService = new ActivityService(configuration.ConnectionStrings.SqlDbConnectionString, log);
            var stravaService = new StravaService();

            var activities = stravaService.GetActivitiesFromCurrentMonth(accessToken);
            foreach (var activity in activities)
            {
                await activityService.InsertAsync(activity);
            }
        }
    }
}

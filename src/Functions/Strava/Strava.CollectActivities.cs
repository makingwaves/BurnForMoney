using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivities
    {
        private static ConfigurationRoot _configuration;
        private static string _accessTokenEncryptionKey;

        [FunctionName("CollectStravaActivitiesInEvery20Minutes")]
        public static async Task RunCollectStravaActivitiesInEvery20MinutesAsync([TimerTrigger("0 */2 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"CollectStravaActivitiesInEvery20Minutes timer trigger processed a request at {DateTime.Now}.");

            await LoadSettingsAsync(executionContext).ConfigureAwait(false);
            var instanceId = await starter.StartNewAsync("CollectStravaActivities", null).ConfigureAwait(false);
            log.LogInformation($"Started orchestration with ID = `{instanceId}`.");
        }


        [FunctionName("CollectStravaActivities")]
        public static async Task RunCollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            log.LogInformation("Orchestration function `CollectStravaActivities` received a request.");

            var encryptedAccessTokens = await context.CallActivityAsync<string[]>("GetEncryptedAccessTokens", null).ConfigureAwait(false);
            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync("CollectSingleUserActivities", encryptedAccessTokens[i]);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        [FunctionName("GetEncryptedAccessTokens")]
        public static async Task<string[]> RunGetEncryptedAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"GetEncryptedAccessTokens function processed a request. Instance id: `{activityContext.InstanceId}`");

            var encryptionService = new AccessTokensEncryptionService(log, _accessTokenEncryptionKey);
            var athletesService = new AthleteService(_configuration.ConnectionStrings.SqlDbConnectionString, log, encryptionService);
            var accessTokens = await athletesService.GetAllEncryptedActiveAccessTokensAsync().ConfigureAwait(false);
            log.LogInformation($"Received information about {accessTokens.Count} active access tokens.");

            return accessTokens.ToArray();
        }

        [FunctionName("CollectSingleUserActivities")]
        public static async Task RunCollectSingleUserActivitiesAsync([ActivityTrigger]string accessToken, ILogger log, ExecutionContext context)
        {
            log.LogInformation("CollectSingleUserActivities function processed a request.");

            var encryptionService = new AccessTokensEncryptionService(log, _accessTokenEncryptionKey);
            var activityService = new ActivityService(_configuration.ConnectionStrings.SqlDbConnectionString, log);
            var stravaService = new StravaService();

            var decryptedAccessToken = encryptionService.DecryptAccessToken(accessToken);

            var activities = stravaService.GetActivitiesFromCurrentMonth(decryptedAccessToken);
            foreach (var activity in activities)
            {
                await activityService.InsertAsync(activity).ConfigureAwait(false);
            }
        }

        private static async Task LoadSettingsAsync(ExecutionContext context)
        {
            if (_configuration != null)
            {
                return;
            }

            _configuration = new ApplicationConfiguration().GetSettings(context);

            if (string.IsNullOrEmpty(_accessTokenEncryptionKey))
            {
                var keyVaultClient = KeyVaultClientFactory.Create();
                var secret = await keyVaultClient.GetSecretAsync(_configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                    .ConfigureAwait(false);
                _accessTokenEncryptionKey = secret.Value;
            }
        }
    }
}

using System;
using System.Linq;
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
        private static ILogger _log;

        [FunctionName("CollectActivities")]
        public static async Task RunCollectActivitiesAsync([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            _log = log;
            _log.LogInformation($"CollectActivities timer trigger processed a request at {DateTime.Now}.");
            await LoadSettingsAsync(context).ConfigureAwait(false);

            var athletesService = new AthleteService(_configuration.ConnectionStrings.SqlDbConnectionString, log, _accessTokenEncryptionKey);
            var accessTokens = await athletesService.GetAllActiveAccessTokensAsync().ConfigureAwait(false);
            _log.LogInformation($"Received information about {accessTokens.Count} active access tokens.");

            var activityService = new ActivityService(_configuration.ConnectionStrings.SqlDbConnectionString, log);
            var stravaService = new StravaService();

            foreach (var accessToken in accessTokens)
            {
                var activities = stravaService.GetActivitiesFromCurrentMonth(accessToken);

                foreach (var activity in activities)
                {
                    await activityService.InsertAsync(activity).ConfigureAwait(false);
                }
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

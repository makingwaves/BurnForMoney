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
        [FunctionName("CollectStravaActivitiesInEvery20Minutes")]
        public static async Task RunTimer([TimerTrigger("0 */1 * * * *")]TimerInfo timer, ILogger log, [OrchestrationClient]DurableOrchestrationClient starter, ExecutionContext executionContext)
        {
            log.LogInformation($"CollectStravaActivitiesInEvery20Minutes timer trigger processed a request at {DateTime.Now}.");

            //await LoadSettingsAsync(executionContext);
            var instanceId = await starter.StartNewAsync("CollectStravaActivities", null);
            log.LogInformation($"Started orchestration with ID = `{instanceId}`.");
        }


        [FunctionName("CollectStravaActivities")]
        public static async Task CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            log.LogInformation("Orchestration function `CollectStravaActivities` received a request.");

            var encryptedAccessTokens = await context.CallActivityAsync<string[]>("GetAccessTokens", null);
            var tasks = new Task[encryptedAccessTokens.Length];
            for (var i = 0; i < encryptedAccessTokens.Length; i++)
            {
                tasks[i] = context.CallActivityAsync("SaveSingleUserActivities", encryptedAccessTokens[i]);
            }

            await Task.WhenAll(tasks);
        }

        [FunctionName("GetAccessTokens")]
        public static async Task<string[]> GetAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"GetAccessTokens function processed a request. Instance id: `{activityContext.InstanceId}`");

            var configuration = new ApplicationConfiguration().GetSettings(context);
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

        [FunctionName("SaveSingleUserActivities")]
        public static async Task SaveSingleUserActivitiesAsync([ActivityTrigger]string accessToken, ILogger log, ExecutionContext context)
        {
            log.LogInformation("CollectSingleUserActivities function processed a request.");

            var configuration = new ApplicationConfiguration().GetSettings(context);
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

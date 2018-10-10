using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.External.Strava.Api.Model;
using BurnForMoney.Functions.Helpers;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesActivities
    {
        [FunctionName(FunctionsNames.A_GetAccessTokens)]
        public static async Task<string[]> GetAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetAccessTokens} function processed a request. Instance id: `{activityContext.InstanceId}`");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            List<string> accessTokens;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                accessTokens = (await conn.QueryAsync<string>("SELECT AccessToken FROM dbo.[Strava.Athletes] where Active = 1")).ToList();
            }

            log.LogInformation($"Received information about {accessTokens.Count} active access tokens.");
            return accessTokens.ToArray();
        }

        [FunctionName(FunctionsNames.A_RetrieveSingleUserActivities)]
        public static List<StravaActivity> A_RetrieveSingleUserActivities([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_RetrieveSingleUserActivities} function processed a request.");

            var (accessToken, from) = context.GetInput<ValueTuple<string, DateTime>>();

            var stravaService = new StravaService();
            var activities = stravaService.GetActivities(accessToken, from);

            return activities.ToList();
        }

        [FunctionName(FunctionsNames.A_GetLastActivitiesUpdateDate)]
        public static async Task<DateTime?> GetLastActivitiesUpdateDate([ActivityTrigger]string systemName, ILogger log, ExecutionContext context)
        {
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var result = await conn.QueryFirstOrDefaultAsync<DateTime?>("SELECT LastUpdate FROM dbo.[Systems.UpdateHistory] WHERE System=@System", new
                    {
                        System = systemName
                    })
                    .ConfigureAwait(false);
                return result;
            }
        }

        [FunctionName(FunctionsNames.A_SetLastActivitiesUpdateDate)]
        public static async Task SetLastActivitiesUpdateDate([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            var (systemName, lastUpdateDate) = context.GetInput<ValueTuple<string, DateTime?>>();

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("UPDATE dbo.[Systems.UpdateHistory] SET LastUpdate=@LastUpdate WHERE System=@System", new
                {
                    System = systemName,
                    LastUpdate = lastUpdateDate
                }).ConfigureAwait(false);

                if (affectedRows == 0)
                {
                    var insertedRows = await conn.ExecuteAsync("INSERT dbo.[Systems.UpdateHistory] (System, LastUpdate) VALUES (@System, @LastUpdate);", new
                    {
                        System = systemName,
                        LastUpdate = lastUpdateDate
                    }).ConfigureAwait(false);
                    if (insertedRows != 1)
                    {
                        log.LogError($"Last update date of the system: {systemName} cannot be updated.");
                        return;
                    }
                }

                log.LogInformation($"Updated last update date of the system: {systemName}");
            }
        }
    }
}

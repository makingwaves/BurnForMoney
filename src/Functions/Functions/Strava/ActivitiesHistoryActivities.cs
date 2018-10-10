using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class ActivitiesHistoryActivities
    {
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
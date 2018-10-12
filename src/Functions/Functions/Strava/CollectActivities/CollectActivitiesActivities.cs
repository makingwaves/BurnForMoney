using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Athlete = BurnForMoney.Functions.Persistence.DatabaseSchema.Athlete;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesActivities
    {
        [FunctionName(FunctionsNames.A_GetAthletesWithAccessTokens)]
        public static async Task<Athlete[]> A_GetAthletesWithAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetAthletesWithAccessTokens} function processed a request. Instance id: `{activityContext.InstanceId}`");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            List<Athlete> athletes;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                athletes = (await conn.QueryAsync<Athlete>("SELECT * FROM dbo.[Strava.Athletes] where Active = 1")).ToList();
            }

            log.LogInformation($"Received information about {athletes.Count} active athletes.");
            return athletes.ToArray();
        }
    }
}

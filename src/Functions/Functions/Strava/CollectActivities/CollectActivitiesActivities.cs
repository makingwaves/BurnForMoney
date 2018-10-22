using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesActivities
    {
        [FunctionName(FunctionsNames.Strava_A_GetActiveAthletesWithAccessTokens)]
        public static async Task<AthleteWithAccessToken[]> A_GetActiveAthletesWithAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_GetActiveAthletesWithAccessTokens} function processed a request. Instance id: `{activityContext.InstanceId}`");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            List<AthleteWithAccessToken> athletes;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                athletes = (await conn.QueryAsync<AthleteWithAccessToken>(@"SELECT Athlete.Id, Tokens.AccessToken AS EncryptedAccessToken, History.LastUpdate
FROM dbo.Athletes AS Athlete
INNER JOIN dbo.[Strava.AccessTokens] AS Tokens ON (Athlete.Id = Tokens.AthleteId)
LEFT JOIN dbo.[Athletes.UpdateHistory] AS History ON (Athlete.Id = History.AthleteId)
WHERE Active = 1 AND Tokens.ExpiresAt > @DateTo AND Tokens.IsValid=1", new { DateTo = DateTime.UtcNow.AddMinutes(1)} )).ToList();
            }

            log.LogInformation($"Received information about {athletes.Count} active athletes.");
            return athletes.ToArray();
        }
    }

    public class AthleteWithAccessToken
    {
        public int Id { get; set; }
        public string EncryptedAccessToken { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}

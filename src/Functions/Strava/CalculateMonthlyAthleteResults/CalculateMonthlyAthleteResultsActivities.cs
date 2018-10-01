using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsActivities
    {
        [FunctionName(FunctionsNames.A_GetLastMonthActivities)]
        public static async Task<List<Activity>> Run([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            var currentDate = DateTime.UtcNow;

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var result = await conn.QueryAsync<Activity>("SELECT * FROM dbo.[Strava.Activities] WHERE MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year", new
                    {
                        Month = currentDate.Month - 1,
                        currentDate.Year
                    })
                    .ConfigureAwait(false);
                return result.ToList();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Strava.DatabaseSchema;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsActivities
    {
        [FunctionName(FunctionsNames.A_GetLastMonthActivities)]
        public static async Task<List<Activity>> A_GetLastMonthActivities([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetLastMonthActivities} function processed a request. Instance id: `{context.InstanceId}`");
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

        [FunctionName(FunctionsNames.A_StoreAggregatedAthleteResults)]
        public static async Task Run([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_StoreAggregatedAthleteResults} function processed a request. Instance id: `{context.InstanceId}`");

            var activities = context.GetInput<List<Activity>>();
            var aggregatedActivities = activities.GroupBy(key => key.AthleteId, element => element, (key, g) =>
            {
                return new
                {
                    AthleteId = key,
                    Distance = g.Sum(activity => activity.Distance),
                    Time = g.Sum(activity => activity.MovingTime),
                    Points = g.Sum(activity => activity.Points),
                    Activities = g.GroupBy(k => k.Category, el => el, (k, a) =>
                    {
                        var categoryActivities = a.ToList();
                        return new
                        {
                            Category = k,
                            Distance = categoryActivities.Sum(activity => activity.Distance),
                            Time = categoryActivities.Sum(activity => activity.MovingTime),
                            Points = categoryActivities.Sum(activity => activity.Points)
                        };
                    })
                };
            });

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                foreach (var aggregatedActivity in aggregatedActivities)
                {
                    var json = JsonConvert.SerializeObject(aggregatedActivities);
                    var currentDate = DateTime.UtcNow;
                    var previousMonth = currentDate.AddMonths(-1);
                    var result = await conn.ExecuteAsync("INSERT dbo.[Strava.AthleteMonthlyResults] (Date, AthleteId, Results) VALUES(@Date, @AthleteId, @Results); ", new
                    {
                        Date = previousMonth,
                        aggregatedActivity.AthleteId,
                        Results = json
                    })
                    .ConfigureAwait(false);

                    log.LogInformation($"Aggregated statistic for athlete with id: {aggregatedActivity.AthleteId}. {json}");
                }
            }
        }
    }
}
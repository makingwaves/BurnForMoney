using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsActivities
    {
        [FunctionName(FunctionsNames.A_GetActivitiesFromGivenMonth)]
        public static async Task<List<Activity>> A_GetActivitiesFromGivenMonth([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetActivitiesFromGivenMonth} function processed a request. Instance id: `{context.InstanceId}`");

            var date = context.GetInput<DateTime>();

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var result = await conn.QueryAsync<Activity>("SELECT * FROM dbo.[Strava.Activities] WHERE MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year", new
                {
                    date.Month,
                    date.Year
                })
                .ConfigureAwait(false);
                return result.ToList();
            }
        }

        [FunctionName(FunctionsNames.A_StoreAggregatedAthleteResults)]
        public static async Task Run([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_StoreAggregatedAthleteResults} function processed a request. Instance id: `{context.InstanceId}`");

            var input = context.GetInput<A_StoreAggregatedAthleteResults_Input>();
            var aggregatedActivities = input.Activities.GroupBy(key => key.AthleteId, element => element, (key, g) =>
            {
                return new
                {
                    AthleteId = key,
                    Distance = g.Sum(activity => activity.Distance),
                    Time = g.Sum(activity => activity.MovingTime),
                    Points = Convert.ToInt32(g.Sum(activity => activity.Points)),
                    Trainings = g.Count(),
                    Activities = g.GroupBy(k => k.Category, el => el, (k, a) =>
                    {
                        var categoryActivities = a.ToList();
                        return new
                        {
                            Category = k,
                            Distance = categoryActivities.Sum(activity => activity.Distance),
                            Time = categoryActivities.Sum(activity => activity.MovingTime),
                            Points = Convert.ToInt32(categoryActivities.Sum(activity => activity.Points)),
                            Trainings = categoryActivities.Count
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

                    await conn.ExecuteAsync("Strava_AthleteMonthlyResults_Upsert", new
                    {
                        Date = $"{input.Date.Year}/{input.Date.Month}",
                        Results = json
                    }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                    log.LogInformation($"Aggregated statistic for athlete with id: {aggregatedActivity.AthleteId}. {json}");
                }
            }
        }

        public class A_StoreAggregatedAthleteResults_Input
        {
            public List<Activity> Activities { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
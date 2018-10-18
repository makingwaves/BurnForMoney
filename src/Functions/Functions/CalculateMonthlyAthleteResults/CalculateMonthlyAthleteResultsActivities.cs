using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsActivities
    {
        [FunctionName(FunctionsNames.A_GetActivitiesFromGivenMonth)]
        public static async Task<List<Activity>> A_GetActivitiesFromGivenMonth([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetActivitiesFromGivenMonth} function processed a request. Instance id: `{context.InstanceId}`");

            var (month, year) = context.GetInput<(int, int)>();

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var result = await conn.QueryAsync<Activity>("SELECT AthleteId, Distance, MovingTime, Category, Points FROM dbo.[Activities] WHERE MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year", new
                {
                    Month = month,
                    Year = year
                })
                .ConfigureAwait(false);
                return result.ToList();
            }
        }
        
        [FunctionName(FunctionsNames.A_SubmitAthleteMonthlyResults)]
        public static async Task A_SubmitAthleteMonthlyResults([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_SubmitAthleteMonthlyResults} function processed a request. Instance id: `{context.InstanceId}`");

            var (activities, (month, year)) = context.GetInput<ValueTuple<List<Activity>, (int, int)>>();
            var aggregatedActivities = GroupActivitiesByAthlete(activities);

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                foreach (var result in aggregatedActivities)
                {
                    var json = JsonConvert.SerializeObject(result);

                    await conn.ExecuteAsync("MonthlyResultsSnapshots_Upsert", new
                    {
                        Date = $"{year}/{month}",
                        Results = json
                    }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                    log.LogInformation($"Aggregated statistics for athlete with id: {result.AthleteId}. {json}");
                }
            }
        }

        private static IEnumerable<AthleteMonthlyResult> GroupActivitiesByAthlete(IEnumerable<Activity> activities)
        {
            var aggregatedActivities = activities.GroupBy(key => key.AthleteId, element => element, (key, g) =>
            {
                var allSingleAthleteActivities = g.ToList();

                return new AthleteMonthlyResult
                {
                    AthleteId = key,
                    Distance = allSingleAthleteActivities.Sum(activity => activity.Distance),
                    Time = allSingleAthleteActivities.Sum(activity => activity.MovingTime),
                    Points = Convert.ToInt32(allSingleAthleteActivities.Sum(activity => activity.Points)),
                    NumberOfTrainings = allSingleAthleteActivities.Count,
                    Activities = allSingleAthleteActivities.GroupBy(k => k.Category, el => el, (k, a) =>
                    {
                        var categoryActivities = a.ToList();
                        return new AthleteMonthlyResultActivity
                        {
                            Category = k,
                            Distance = categoryActivities.Sum(activity => activity.Distance),
                            Time = categoryActivities.Sum(activity => activity.MovingTime),
                            Points = Convert.ToInt32(categoryActivities.Sum(activity => activity.Points)),
                            NumberOfTrainings = categoryActivities.Count
                        };
                    }).ToList()
                };
            });

            return aggregatedActivities;
        }
    }

    public class Activity
    {
        public int AthleteId { get; set; }
        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }

    public class AthleteMonthlyResult
    {
        public int AthleteId { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
        public List<AthleteMonthlyResultActivity> Activities { get; set; }
    }

    public class AthleteMonthlyResultActivity
    {
        public string Category { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
    }
}
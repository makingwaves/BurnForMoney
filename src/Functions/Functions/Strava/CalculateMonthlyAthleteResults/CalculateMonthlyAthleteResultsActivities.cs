//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Threading.Tasks;
//using BurnForMoney.Functions.Configuration;
//using BurnForMoney.Functions.Persistence.DatabaseSchema;
//using Dapper;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

//namespace BurnForMoney.Functions.Functions.Strava.CalculateMonthlyAthleteResults
//{
//    public static class CalculateMonthlyAthleteResultsActivities
//    {
//        [FunctionName(FunctionsNames.A_GetActivitiesFromGivenMonth)]
//        public static async Task<List<Activity>> A_GetActivitiesFromGivenMonth([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
//        {
//            log.LogInformation($"{FunctionsNames.A_GetActivitiesFromGivenMonth} function processed a request. Instance id: `{context.InstanceId}`");

//            var (month, year) = context.GetInput<(int, int)>();

//            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
//            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
//            {
//                var result = await conn.QueryAsync<Activity>("SELECT * FROM dbo.[Strava.Activities] WHERE MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year", new
//                {
//                    Month = month,
//                    Year = year
//                })
//                .ConfigureAwait(false);
//                return result.ToList();
//            }
//        }

//        [FunctionName(FunctionsNames.A_GroupActivitiesByAthlete)]
//        public static List<AthleteMonthlyResult> A_GroupActivitiesByAthlete([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
//        {
//            log.LogInformation($"{FunctionsNames.A_GroupActivitiesByAthlete} function processed a request. Instance id: `{context.InstanceId}`");

//            var activities = context.GetInput<List<Activity>>();
//            var aggregatedActivities = activities.GroupBy(key => key.AthleteId, element => element, (key, g) =>
//            {
//                var allSingleAthleteActivities = g.ToList();

//                return new AthleteMonthlyResult
//                {
//                    AthleteId = key,
//                    Distance = allSingleAthleteActivities.Sum(activity => activity.Distance),
//                    Time = allSingleAthleteActivities.Sum(activity => activity.MovingTime),
//                    Points = Convert.ToInt32(allSingleAthleteActivities.Sum(activity => activity.Points)),
//                    NumberOfTrainings = allSingleAthleteActivities.Count,
//                    Activities = allSingleAthleteActivities.GroupBy(k => k.Category, el => el, (k, a) =>
//                    {
//                        var categoryActivities = a.ToList();
//                        return new AthleteMonthlyResultActivity
//                        {
//                            Category = k,
//                            Distance = categoryActivities.Sum(activity => activity.Distance),
//                            Time = categoryActivities.Sum(activity => activity.MovingTime),
//                            Points = Convert.ToInt32(categoryActivities.Sum(activity => activity.Points)),
//                            NumberOfTrainings = categoryActivities.Count
//                        };
//                    }).ToList()
//                };
//            });

//            return aggregatedActivities.ToList();
//        }

//        [FunctionName(FunctionsNames.A_StoreAggregatedAthleteMonthlyResults)]
//        public static async Task A_StoreAggregatedAthleteMonthlyResults([ActivityTrigger] DurableActivityContext context, ILogger log, ExecutionContext executionContext)
//        {
//            log.LogInformation($"{FunctionsNames.A_StoreAggregatedAthleteMonthlyResults} function processed a request. Instance id: `{context.InstanceId}`");

//            var ( activities, (month, year)) = context.GetInput<ValueTuple<List<AthleteMonthlyResult>, (int, int)>>();
//            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
//            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
//            {
//                foreach (var aggregatedActivity in activities)
//                {
//                    var json = JsonConvert.SerializeObject(activities);

//                    await conn.ExecuteAsync("Strava_AthleteMonthlyResults_Upsert", new
//                    {
//                        Date = $"{year}/{month}",
//                        Results = json
//                    }, commandType: CommandType.StoredProcedure)
//                    .ConfigureAwait(false);

//                    log.LogInformation($"Aggregated statistics for athlete with id: {aggregatedActivity.AthleteId}. {json}");
//                }
//            }
//        }
//    }
//}
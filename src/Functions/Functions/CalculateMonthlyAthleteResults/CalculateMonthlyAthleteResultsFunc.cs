using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults
{
    public static class CalculateMonthlyAthleteResultsFunc
    {
        [FunctionName(FunctionsNames.Q_CalculateMonthlyAthleteResults)]
        public static async Task Q_CalculateMonthlyAthleteResults([QueueTrigger(QueueNames.CalculateMonthlyResults)] CalculateMonthlyResultsRequest request, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Q_CalculateMonthlyAthleteResults} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var activities = (await conn.QueryAsync<Activity>(
                        @"SELECT AthleteId, Athletes.FirstName as AthleteFirstName, Athletes.LastName as AthleteLastName, Distance, MovingTime, Category, Points 
FROM dbo.[Activities] AS Activities
INNER JOIN dbo.[Athletes] AS Athletes ON (Activities.AthleteId = Athletes.Id)
WHERE MONTH(ActivityTime)=@Month AND YEAR(ActivityTime)=@Year", new
                        {
                            request.Month,
                            request.Year
                        })
                    .ConfigureAwait(false)).ToList();

                if (!activities.Any())
                {
                    log.LogWarning($"[{FunctionsNames.Q_CalculateMonthlyAthleteResults}] cannot find any activities from given date: {request.Month}/{request.Year}.");
                    return;
                }

                var aggregatedActivities = GroupActivitiesByAthlete(activities);

                var json = JsonConvert.SerializeObject(aggregatedActivities);

                var affectedRows = await conn.ExecuteAsync("MonthlyResultsSnapshots_Upsert", new
                    {
                        Date = $"{request.Year}/{request.Month}",
                        Results = json
                    }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);
                if (affectedRows == 1)
                {
                    log.LogInformation($"[{FunctionsNames.Q_CalculateMonthlyAthleteResults}] Updated snapshot.");
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
                    AthleteName = $"{allSingleAthleteActivities[0].AthleteFirstName} {allSingleAthleteActivities[0].AthleteLastName}",
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
        public string AthleteFirstName { get; set; }
        public string AthleteLastName { get; set; }
        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }

    public class AthleteMonthlyResult
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; }
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
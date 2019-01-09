using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Presentation.Configuration;
using BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots
{
    public static class CalculateMonthlyAthleteResultsFunc
    {
        // ReSharper disable once InconsistentNaming
        public const string FUNCTIONNAME_Q_CalculateMonthlyAthleteResults = "Q_CalculateMonthlyAthleteResults";

        [FunctionName(FUNCTIONNAME_Q_CalculateMonthlyAthleteResults)]
        public static async Task Q_CalculateMonthlyAthleteResults([QueueTrigger(QueueNames.CalculateMonthlyResults)] CalculateMonthlyResultsRequest request, 
            ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

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
                    log.LogWarning(FUNCTIONNAME_Q_CalculateMonthlyAthleteResults, $"Cannot find any activities from given date: {request.Month}/{request.Year}.");
                    return;
                }

                var athleteResults = GroupActivitiesByAthlete(activities);
                var monthResults = new AthleteMonthlyResult
                {
                    Points = Convert.ToInt32(activities.Sum(a => a.Points)),
                    Distance = activities.Sum(a => a.Distance),
                    Time = activities.Sum(a => a.MovingTime),
                    AthleteResults = athleteResults.ToList()
                };

                var json = JsonConvert.SerializeObject(monthResults);
                var affectedRows = await conn.ExecuteAsync("MonthlyResultsSnapshots_Upsert", new
                    {
                        Date = $"{request.Year}/{request.Month}",
                        Results = json
                    }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);
                if (affectedRows == 1)
                {
                    log.LogInformation(FUNCTIONNAME_Q_CalculateMonthlyAthleteResults, "Updated snapshot.");
                }
            }
        }

        private static IEnumerable<AthleteResult> GroupActivitiesByAthlete(IEnumerable<Activity> activities)
        {
            var athleteResults = activities.GroupBy(key => key.AthleteId, element => element, (key, g) =>
            {
                var allSingleAthleteActivities = g.ToList();

                return new AthleteResult
                {
                    Id = key,
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

            return athleteResults;
        }
    }

    public class Activity
    {
        public Guid AthleteId { get; set; }
        public string AthleteFirstName { get; set; }
        public string AthleteLastName { get; set; }
        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public string Category { get; set; }
        public double Points { get; set; }
    }
}
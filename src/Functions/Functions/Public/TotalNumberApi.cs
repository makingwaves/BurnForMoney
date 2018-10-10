using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Persistence.DatabaseSchema;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Public
{
    public static class TotalNumberApi
    {
        [FunctionName("TotalNumbers")]
        public static async Task<IActionResult> TotalNumbers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "totalnumbers")]HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var jsonResults = await conn.QueryAsync<(string date, string json)>("SELECT Date, Results FROM dbo.[Strava.AthleteMonthlyResults]")
                    .ConfigureAwait(false);

                var jsons = jsonResults.Select(record =>
                    new
                    {
                        Date = record.date,
                        Results = JsonConvert.DeserializeObject<List<AthleteMonthlyResult>>(record.json)
                    })
                    .ToList();

                var totalDistance = jsons.Sum(j => j.Results.Sum(r => r.Distance));
                var totalTime = jsons.Sum(j => j.Results.Sum(r => r.Time));
                var totalPoints = jsons.Sum(j => j.Results.Sum(r => r.Points));

                var thisMonth = jsons.Last();
                var totalPointsThisMonth = thisMonth.Results.Sum(r => r.Points);
                var mostFrequentActivities = thisMonth.Results.SelectMany(r => r.Activities)
                    .GroupBy(key => key.Category, el => el, (category, activities) =>
                    {
                        activities = activities.ToList();
                        return new
                        {
                            Category = category,
                            NumberOfTrainings = activities.Sum(a => a.NumberOfTrainings),
                            Points = activities.Sum(a => a.Points)
                        };
                    }).OrderByDescending(o => o.NumberOfTrainings)
                    .Take(5);


                var result = new
                {
                    Distance = totalDistance,
                    Time = totalTime,
                    Money = totalPoints * 100 / 500,
                    ThisMonth = new
                    {
                        NumberOfTrainings = jsons.Sum(j => j.Results.Sum(r => r.NumberOfTrainings)),
                        PercentOfEngagedEmployees = 37,
                        Money = totalPointsThisMonth * 100 / 500,
                        MostFrequentActivities = mostFrequentActivities
                    }
                };

                return new OkObjectResult(result);
            };
        }
    }
}
using System.Collections.Generic;
using System.Data;
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
            var configuration = ApplicationConfiguration.GetSettings(executionContext);
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
                var totalPointsThisMonth = jsons.Last().Results.Sum(r => r.Points);

                var result = new
                {
                    Distance = totalDistance,
                    Time = totalTime,
                    Money = totalPoints * 100 / 500,
                    NumberOfTrainings = jsons.Sum(j => j.Results.Sum(r => r.NumberOfTrainings)),
                    PercentOfEngagedEmployees = 37,
                    MoneyEarnedThisMonth = totalPointsThisMonth * 100 / 500,
                    MostFrequentActivities = new[]
                    {
                        new
                        {
                            Category = "Ride",
                            Points = 250
                        },
                        new
                        {
                            Category = "Running",
                            Points = 210
                        },
                        new
                        {
                            Category = "Gym",
                            Points = 57
                        },
                        new
                        {
                            Category = "Team sports",
                            Points = 55
                        },
                        new
                        {
                            Category = "Fitness",
                            Points = 10
                        }
                    }
                };

                return new OkObjectResult(result);
            };
        }
    }
}
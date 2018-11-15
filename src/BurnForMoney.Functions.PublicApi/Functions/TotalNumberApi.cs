using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.PublicApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.PublicApi.Functions
{
    public static class TotalNumberApi
    {
        private const string CacheKey = "api.totalnumbers";
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryDistributedCacheOptions());

        [FunctionName("TotalNumbers")]
        public static async Task<IActionResult> TotalNumbers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "totalnumbers")] HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart("TotalNumbers");
            if (!Cache.TryGetValue(CacheKey, out var totalNumbers))
            {
                totalNumbers = await GetTotalNumbersAsync(executionContext);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    Size = 1,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                Cache.Set(CacheKey, totalNumbers, cacheEntryOptions);
            }

            log.LogFunctionEnd("TotalNumbers");
            return new OkObjectResult(totalNumbers);
        }

        private static async Task<object> GetTotalNumbersAsync(ExecutionContext executionContext)
        {
            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = SqlConnectionFactory.CreateWithRetry(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var jsonResults = await conn.QueryAsync<(string date, string json)>("SELECT Date, Results FROM dbo.[MonthlyResultsSnapshots]")
                    .ConfigureAwait(false);

                var months = jsonResults.Select(record =>
                    new
                    {
                        Date = record.date,
                        Results = JsonConvert.DeserializeObject<List<AthleteMonthlyResult>>(record.json)
                    })
                    .OrderBy(month => month.Date, new DateComparer())
                    .ToList();

                var totalDistance = months.Sum(j => j.Results.Sum(r => r.Distance));
                var totalTime = months.Sum(j => j.Results.Sum(r => r.Time));
                var totalPoints = months.Sum(j => j.Results.Sum(r => r.Points));

                var thisMonth = months.Last();
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

                var uniqueAthletes = thisMonth.Results.Count;

                var result = new
                {
                    Distance = (int)UnitsConverter.ConvertMetersToKilometers(totalDistance, 0),
                    Time = (int)UnitsConverter.ConvertMinutesToHours(totalTime, 0),
                    Money = PointsToMoneyConverter.Convert(totalPoints),
                    ThisMonth = new
                    {
                        NumberOfTrainings = thisMonth.Results.Sum(r => r.NumberOfTrainings),
                        PercentOfEngagedEmployees = EmployeesEngagementCalculator.GetPercentOfEngagedEmployees(uniqueAthletes),
                        Points = totalPointsThisMonth,
                        Money = PointsToMoneyConverter.Convert(totalPointsThisMonth),
                        MostFrequentActivities = mostFrequentActivities
                    }
                };

                return result;
            }
        }

        public class EmployeesEngagementCalculator
        {
            public const int NumberOfEmployees = 97;

            public static int GetPercentOfEngagedEmployees(int numberOfTheUniqueAthletes) =>
                (numberOfTheUniqueAthletes * 100) / NumberOfEmployees;
        }

        public class PointsToMoneyConverter
        {
            public static int Convert(int points) => points * 100 / 500;
        }
    }

    public class DateComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xSplit = x.Split('/');
            var xDate = new DateTime(int.Parse(xSplit[0]), int.Parse(xSplit[1]), 1, 0, 0, 0);
            var ySplit = y.Split('/');
            var yDate = new DateTime(int.Parse(ySplit[0]), int.Parse(ySplit[1]), 1, 0, 0, 0);
            return xDate.CompareTo(yDate);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.PublicApi.Configuration;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
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
        private static EmployeesEngagementCalculator _employeesEngagementCalculator;

        [FunctionName(FunctionNameConvention.HttpTriggerPrefix + "TotalNumbers")]
        public static async Task<IActionResult> TotalNumbers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "totalnumbers")] HttpRequest req, 
            ILogger log, [Configuration] ConfigurationRoot configuration)
        {
            _employeesEngagementCalculator = new EmployeesEngagementCalculator(configuration.CompanyInformation.NumberOfEmployees);

            if (!Cache.TryGetValue(CacheKey, out var totalNumbers))
            {
                totalNumbers = await GetTotalNumbersAsync(configuration.ConnectionStrings.SqlDbConnectionString);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    Size = 1,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                Cache.Set(CacheKey, totalNumbers, cacheEntryOptions);
            }

            return new OkObjectResult(totalNumbers);
        }

        private static async Task<object> GetTotalNumbersAsync(string connectionString)
        {
            var today = DateTime.UtcNow;

            using (var conn = SqlConnectionFactory.Create(connectionString))
            {
                await conn.OpenWithRetryAsync();

                var dto = await conn.QueryAsync<(string date, string json)>("SELECT Date, Results FROM dbo.[MonthlyResultsSnapshots]")
                    .ConfigureAwait(false);

                var results = dto.Select(record =>
                    new
                    {
                        Date = record.date,
                        Results = JsonConvert.DeserializeObject<AthleteMonthlyResult>(record.json)
                    })
                    .OrderBy(month => month.Date, new DateComparer())
                    .ToList();

                var totalDistance = results.Sum(r => r.Results.Distance);
                var totalTime = results.Sum(r => r.Results.Time);
                var totalPoints = results.Sum(r => r.Results.Points);

                var thisMonth = results.SingleOrDefault(r => r.Date.Equals($"{today.Year}/{today.Month}"));

                var result = new
                {
                    Distance = (int)UnitsConverter.ConvertMetersToKilometers(totalDistance, 0),
                    Time = (int)UnitsConverter.ConvertMinutesToHours(totalTime, 0),
                    Money = PointsToMoneyConverter.Convert(totalPoints),
                    ThisMonth = thisMonth == null ? ThisMonth.NoResults : GetThisMonthStatistics(thisMonth.Results)
                };

                return result;
            }
        }

        private static ThisMonth GetThisMonthStatistics(AthleteMonthlyResult thisMonth)
        {
            var uniqueAthletes = thisMonth.AthleteResults.Count;

            var totalPointsThisMonth = thisMonth.Points;
            var mostFrequentActivities = thisMonth
                .AthleteResults
                .SelectMany(r => r.Activities)
                .GroupBy(key => key.Category, el => el, (category, activities) =>
                {
                    activities = activities.ToList();
                    return new
                    {
                        Category = category,
                        NumberOfTrainings = activities.Sum(a => a.NumberOfTrainings),
                        Points = activities.Sum(a => a.Points)
                    };
                })
                .OrderByDescending(o => o.NumberOfTrainings)
                .Take(5);

            return new ThisMonth
            {
                NumberOfTrainings = thisMonth.AthleteResults.Sum(r => r.NumberOfTrainings),
                PercentOfEngagedEmployees = _employeesEngagementCalculator.GetPercentOfEngagedEmployees(uniqueAthletes),
                Points = totalPointsThisMonth,
                Money = PointsToMoneyConverter.Convert(totalPointsThisMonth),
                MostFrequentActivities = mostFrequentActivities
            };
        }

        private class EmployeesEngagementCalculator
        {
            private readonly int _numberOfEmployees;

            public EmployeesEngagementCalculator(int numberOfEmployees)
            {
                _numberOfEmployees = numberOfEmployees;
            }

            public int GetPercentOfEngagedEmployees(int numberOfTheUniqueAthletes) =>
                (numberOfTheUniqueAthletes * 100) / _numberOfEmployees;
        }

        public class PointsToMoneyConverter
        {
            public static int Convert(int points) => points * 100 / 500;
        }

        public class ThisMonth
        {
            public static ThisMonth NoResults = new ThisMonth
            {
                NumberOfTrainings = 0,
                PercentOfEngagedEmployees = 0,
                Points = 0,
                Money = 0,
                MostFrequentActivities = new List<object>()
            };
            public int NumberOfTrainings { get; set; }
            public int PercentOfEngagedEmployees { get; set; }
            public int Points { get; set; }
            public int Money { get; set; }
            public IEnumerable<object> MostFrequentActivities { get; set; }
        }
    }

    public class DateComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            var xSplit = x.Split('/');
            var xDate = new DateTime(int.Parse(xSplit[0]), int.Parse(xSplit[1]), 1, 0, 0, 0);
            var ySplit = y.Split('/');
            var yDate = new DateTime(int.Parse(ySplit[0]), int.Parse(ySplit[1]), 1, 0, 0, 0);
            return xDate.CompareTo(yDate);
        }
    }
}
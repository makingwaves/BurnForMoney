using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;

namespace BurnForMoney.Functions.Shared.Repositories
{
    public class ActivityRepository
    {
        private readonly string _sqlConnectionString;

        public ActivityRepository(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task<Activity> GetByExternalIdAsync(string externalId)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = await conn.QuerySingleAsync<Activity>(
                    "SELECT * FROM dbo.Activites WHERE ExternalId=@ExternalId", new
                    {
                        ExternalId = externalId
                    });
                return activity;
            }
        }
    }

    public class Activity
    {
        public string AthleteId { get; set; }
        public string ActivityId { get; set; }
        public string ExternalId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }

        public string ActivityType { get; set; }
        public DateTime StartDate { get; set; }
        public string Source { get; set; }
    }
}
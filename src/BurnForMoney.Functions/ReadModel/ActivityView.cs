using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Infrastructure.Events;
using Dapper;

namespace BurnForMoney.Functions.ReadModel
{
    public class ActivityView : IHandles<ActivityAdded>, IHandles<ActivityDeleted>, IHandles<ActivityUpdated>
    {
        private readonly string _sqlConnectionString;

        public ActivityView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(ActivityAdded message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                await conn.ExecuteAsync(@"INSERT INTO dbo.Activities 
(Id, AthleteId, ExternalId, ActivityTime, ActivityType, Distance, MovingTime, Source) 
VALUES (@Id, @AthleteId, @ExternalId, @ActivityTime, @ActivityType, @Distance, @MovingTime, @Source)", new
                {
                    Id = message.ActivityId,
                    AthleteId = message.AthleteId,
                    ExternalId = message.ExternalId,
                    ActivityTime = message.StartDate,
                    ActivityType = message.ActivityType,
                    Distance = message.DistanceInMeters,
                    MovingTime = message.MovingTimeInMinutes,
                    Source = message.Source
                });
            }
        }

        public async Task HandleAsync(ActivityUpdated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                await conn.ExecuteAsync(@"UPDATE dbo.Activities
SET ActivityTime=@ActivityTime, ActivityType=@ActivityType, Distance=@Distance, MovingTime=@MovingTime
WHERE Id=@Id", new
                {
                    Id = message.ActivityId,
                    ActivityTime = message.StartDate,
                    ActivityType = message.ActivityType,
                    Distance = message.DistanceInMeters,
                    MovingTime = message.MovingTimeInMinutes
                });
            }
        }

        public async Task HandleAsync(ActivityDeleted message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                await conn.ExecuteAsync("DELETE FROM dbo.Activities WHERE Id=@Id", new
                {
                    Id = message.ActivityId
                });
            }
        }
    }
}
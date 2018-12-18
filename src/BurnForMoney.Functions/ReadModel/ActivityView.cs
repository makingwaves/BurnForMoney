using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Shared.Persistence;
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
(Id, AthleteId, ExternalId, ActivityTime, ActivityType, Category, Distance, MovingTime, Source, Points) 
VALUES (@Id, @AthleteId, @ExternalId, @ActivityTime, @ActivityType, @ActivityCategory, @Distance, @MovingTime, @Source, @Points)", new
                {
                    Id = message.ActivityId,
                    message.AthleteId,
                    message.ExternalId,
                    ActivityTime = message.StartDate,
                    message.ActivityType,
                    ActivityCategory = message.ActivityCategory.ToString(),
                    Distance = message.DistanceInMeters,
                    MovingTime = message.MovingTimeInMinutes,
                    Source = message.Source.ToString(),
                    message.Points
                });
            }
        }

        public async Task HandleAsync(ActivityUpdated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                await conn.ExecuteAsync(@"UPDATE dbo.Activities
SET ActivityTime=@ActivityTime, ActivityType=@ActivityType, Category=@ActivityCategory, Distance=@Distance, MovingTime=@MovingTime, Points=@Points
WHERE Id=@Id", new
                {
                    Id = message.ActivityId,
                    ActivityTime = message.StartDate,
                    message.ActivityType,
                    ActivityCategory = message.ActivityCategory.ToString(),
                    Distance = message.DistanceInMeters,
                    MovingTime = message.MovingTimeInMinutes,
                    message.Points
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
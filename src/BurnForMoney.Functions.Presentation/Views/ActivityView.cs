using System;
using System.Data;
using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Exceptions;
using BurnForMoney.Functions.Presentation.Views.Poco;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using DapperExtensions;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class ActivityView : IHandles<ActivityAdded>, IHandles<ActivityDeleted_V2>, IHandles<ActivityUpdated_V2>
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

                var row = new Activity
                {
                    Id = message.ActivityId,
                    AthleteId = message.AthleteId,
                    ExternalId = message.ExternalId,
                    ActivityTime = message.StartDate,
                    ActivityType = message.ActivityType,
                    Category = message.ActivityCategory.ToString(),
                    Distance = message.DistanceInMeters,
                    MovingTime = Convert.ToInt32(message.MovingTimeInMinutes),
                    Source = message.Source.ToString(),
                    Points = message.Points
                };
                var inserted = conn.Insert(row);
                if (inserted == null)
                {
                    throw new FailedToCreateActivityException(message.ActivityId);
                }
            }
        }

        public async Task HandleAsync(ActivityUpdated_V2 message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = conn.Get<Activity>(message.ActivityId);
                activity.ActivityTime = message.StartDate;
                activity.ActivityType = message.ActivityType;
                activity.Category = message.ActivityCategory.ToString();
                activity.Distance = message.DistanceInMeters;
                activity.MovingTime = Convert.ToInt32(message.MovingTimeInMinutes);
                activity.Points = message.Points;

                var success = conn.Update(activity);

                if (!success)
                {
                    throw new FailedToUpdateActivityException(message.ActivityId);
                }
            }
        }

        public async Task HandleAsync(ActivityDeleted_V2 message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = conn.Get<Activity>(message.ActivityId);
                var success = conn.Delete(activity);

                if (!success)
                {
                    throw new FailedToDeleteActivityException(message.ActivityId);
                }
            }
        }
    }
}
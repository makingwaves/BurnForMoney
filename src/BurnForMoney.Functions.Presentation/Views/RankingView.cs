using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Exceptions;
using BurnForMoney.Functions.Presentation.Views.Mappers;
using BurnForMoney.Functions.Presentation.Views.Poco;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class RankingView : IHandles<ActivityAdded>, IHandles<ActivityDeleted>, IHandles<ActivityUpdated>
    {
        private readonly string _sqlConnectionString;

        public RankingView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(ActivityAdded message)
        {
            await InsertOrUpdateAsync(message.AthleteId, message.ActivityCategory.ToString(), message.StartDate.Month,
                message.StartDate.Year, 
                Convert.ToInt32(message.DistanceInMeters), 
                Convert.ToInt32(message.MovingTimeInMinutes), 
                message.Points);
        }

        public async Task InsertOrUpdateAsync(Guid athleteId, string category, int month, int year,
            int distance, int movingTime, double points)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();
            
                var result = conn.Get<IndividualResult>(
                        new {
                            AthleteId = athleteId,
                            Category = category,
                            Month = month,
                            Year = year
                        });

                if (result == null)
                {
                    var row = new IndividualResult
                    {
                        AthleteId = athleteId,
                        Category = category,
                        Month = month,
                        Year = year,
                        Distance = distance,
                        MovingTime = movingTime,
                        Points = points
                    };
                    var inserted = conn.Insert(row);
                    if (inserted == null)
                    {
                        throw new DataException($"Failed to insert a new IndividualResult of athlete: {athleteId}.");
                    }
                }
                else
                {
                    result.Distance += distance;
                    result.MovingTime += movingTime;
                    result.Points += points;

                    var affectedRows = await conn.ExecuteAsync($@"UPDATE dbo.[{IndividualResultMapper.TableName}] 
                    SET Distance=@Distance, MovingTime=@MovingTime, Points=@Points, Version=@Version
                    WHERE Version=@PreviousVersion AND AthleteId=@AthleteId AND Category=@Category AND Month=@Month AND Year=@Year", new {
                        result.Distance,
                        result.MovingTime,
                        result.Points,
                        PreviousVersion = result.Version,
                        Version = ++result.Version,
                        result.AthleteId,
                        result.Category,
                        result.Month,
                        result.Year
                    });
                    if (affectedRows == 0)
                    {
                        throw new SqlConcurrencyException();
                    }
                }
            }
        }

        public Task HandleAsync(ActivityDeleted message)
        {
            return Task.CompletedTask;
            /* 
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = conn.Get<Activity>(message.ActivityId);
                var row = conn.Get<IndividualResult>(new { 
                    AthleteId = activity.AthleteId,
                    Category = message.ActivityCategory.ToString(),
                    Month = message.StartDate.Month,
                    Year = message.StartDate.Year
                });

                row.Distance -= Convert.ToInt32(message.DistanceInMeters);
                row.MovingTime -= Convert.ToInt32(message.MovingTimeInMinutes);
                row.Points -= message.Points;

                await UpdateRowAsync(row);
            }
            */
        }

        public Task HandleAsync(ActivityUpdated message)
        {
            return Task.CompletedTask;

            /* 
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activity = conn.Get<Activity>(message.ActivityId);
                var row = conn.Get<IndividualResult>(new { 
                    AthleteId = activity.AthleteId,
                    Category = activity.Category.ToString(),
                    Month = activity.ActivityTime.Month,
                    Year = activity.ActivityTime.Year
                });

                row.Distance += Convert.ToInt32(message.DistanceInMeters); // calculate delta
                row.MovingTime += Convert.ToInt32(message.MovingTimeInMinutes); // calculate delta
                row.Points += message.Points; // calculate delta

                await UpdateRowAsync(row);
            }
            */
        }
    }
}
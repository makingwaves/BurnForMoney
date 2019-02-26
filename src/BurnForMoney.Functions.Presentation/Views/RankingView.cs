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
    public class RankingView : IHandles<ActivityAdded>, IHandles<ActivityDeleted_V2>, IHandles<ActivityUpdated_V2>
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

        public async Task HandleAsync(ActivityDeleted_V2 message)
        {
            await InsertOrUpdateAsync(message.AthleteId, message.PreviousData.ActivityCategory.ToString(), 
                message.PreviousData.StartDate.Month,
                message.PreviousData.StartDate.Year, 
                Convert.ToInt32(message.PreviousData.DistanceInMeters) * -1, 
                Convert.ToInt32(message.PreviousData.MovingTimeInMinutes) * -1,
                message.PreviousData.Points * -1);
        }

        public async Task HandleAsync(ActivityUpdated_V2 message)
        {
            var deltaDistance = Convert.ToInt32(message.DistanceInMeters - message.PreviousData.DistanceInMeters);
            var deltaMovingTime = Convert.ToInt32(message.MovingTimeInMinutes - message.PreviousData.MovingTimeInMinutes);
            var deltaPoints = message.Points - message.PreviousData.Points;

            await InsertOrUpdateAsync(message.AthleteId, message.ActivityCategory.ToString(), 
                message.StartDate.Month,
                message.StartDate.Year, 
                deltaDistance, 
                deltaMovingTime,
                deltaPoints);
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
    }
}
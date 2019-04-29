﻿using System;
using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Exceptions;
using BurnForMoney.Functions.Presentation.Views.Poco;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using DapperExtensions;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class AthleteView : IHandles<AthleteCreated>, IHandles<AthleteDeactivated>, IHandles<AthleteActivated>
    {
        private readonly string _sqlConnectionString;

        public AthleteView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(AthleteCreated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var row = new Athlete
                {
                    Id = message.Id,
                    ExternalId = message.ExternalId,
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    ProfilePictureUrl = message.ProfilePictureUrl,
                    Active = true,
                    System = message.System.ToString()
                };
                var inserted = conn.Insert(row);
                if (inserted == null)
                {
                    throw new FailedToAddAthleteException(message.Id);
                }
            }
        }
        
        public async Task HandleAsync(AthleteCreated_V2 message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var row = new Athlete
                {
                    Id = message.Id,                    
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    ProfilePictureUrl = string.Empty,
                    Active = true,                   
                    System = "None"
                };

                var inserted = conn.Insert(row);
                if (inserted == null)
                {
                    throw new FailedToAddAthleteException(message.Id);
                }
            }
        }

        public async Task HandleAsync(StravaIdAdded message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(
                    @"UPDATE dbo.Athletes SET ExternalId=@stravaId WHERE Id=@Id",
                    new { Id = message.AthleteId, stravaId = message.StravaId.ToString() });

                if (affectedRows != 1)                
                    throw new FailedToAddStravaAccountException(message.AthleteId, message.StravaId);
            }
        }
        
        [Obsolete("Saved for legacy reasons, will be removed after events migration.")]
        public async Task HandleAsync(ActiveDirectoryIdAssigned message)
        {
        }

        public async Task HandleAsync(AthleteDeactivated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=0 WHERE Id=@Id", new { Id = message.AthleteId});

                if (affectedRows != 1)
                {
                    throw new FailedToDeactivateAthleteException(message.AthleteId);
                }
            }
        }

        public async Task HandleAsync(AthleteActivated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=1 WHERE Id=@Id", new { Id = message.AthleteId });

                if (affectedRows != 1)
                {
                    throw new FailedToActivateAthleteException(message.AthleteId);
                }
            }
        }
    }
}
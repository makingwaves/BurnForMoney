using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;

namespace BurnForMoney.Infrastructure.Persistence.Repositories
{
    public class AthleteReadRepository : IReadFacade<AthleteRow>
    {
        private readonly string _connectionString;

        public AthleteReadRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<IEnumerable<AthleteRow>> GetAllActiveAsync()
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var athletes = await conn.QueryAsync<AthleteRow>(
                        "SELECT Id, ExternalId, FirstName, LastName, ProfilePictureUrl, System, Active FROM dbo.Athletes WHERE Active=1");

                return athletes;
            }
        }

        public async Task<bool> AthleteWithStravaIdExistsAsync(string id)
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var exists = await conn.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM dbo.Athletes WHERE ExternalId=@ExternalId", new
                {
                    ExternalId = id
                });
                return exists;
            }
        }

        public async Task<AthleteRow> GetAthleteByIdAsync(Guid id)
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id, FirstName, LastName, Active FROM dbo.Athletes WHERE Id=@Id", new
                {
                    Id = id
                });

                if (!athlete.Active)
                {
                    return AthleteRow.NonActive;
                }

                return athlete;
            }
        }

        public async Task<AthleteRow> GetAthleteByStravaIdAsync(string id)
        {
            using (var conn = SqlConnectionFactory.Create(_connectionString))
            {
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id, FirstName, LastName, Active FROM dbo.Athletes WHERE ExternalId=@ExternalId", new
                {
                    ExternalId = id
                });

                if (!athlete.Active)
                {
                    return AthleteRow.NonActive;
                }

                return athlete;
            }
        }
    }
}
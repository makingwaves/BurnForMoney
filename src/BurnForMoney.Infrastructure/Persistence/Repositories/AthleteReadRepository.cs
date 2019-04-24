using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;

namespace BurnForMoney.Infrastructure.Persistence.Repositories
{
    public interface IAthleteReadRepository
    {
        Task<IEnumerable<AthleteRow>> GetAllActiveAsync();
        Task<bool> AthleteWithStravaIdExistsAsync(string id);
        Task<AthleteRow> GetAthleteByIdAsync(Guid id);
        Task<AthleteRow> GetAthleteByStravaIdAsync(string id);
        Task<AthleteRow> GetAthleteByAadIdAsync(Guid aadId);
    }

    public class AthleteReadRepository : IAthleteReadRepository
    {
        private readonly IConnectionProvider<SqlConnection> _connectionProvider;
        private readonly IAccountsStore _accountsStore;

        public AthleteReadRepository(IConnectionProvider<SqlConnection> connectionProvider,
            IAccountsStore accountsStore)
        {
            _connectionProvider = connectionProvider;
            _accountsStore = accountsStore;
        }

        public async Task<IEnumerable<AthleteRow>> GetAllActiveAsync()
        {
            using (var conn = _connectionProvider.Create())
            {
                await conn.OpenWithRetryAsync();

                var athletes = await conn.QueryAsync<AthleteRow>(
                        "SELECT Id, ExternalId, FirstName, LastName, ProfilePictureUrl, System, Active FROM dbo.Athletes WHERE Active=1");

                return athletes;
            }
        }

        public async Task<bool> AthleteWithStravaIdExistsAsync(string id)
        {
            using (var conn = _connectionProvider.Create())
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
            using (var conn = _connectionProvider.Create())
            {
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id, FirstName, LastName, Active FROM dbo.Athletes WHERE Id=@Id", new
                {
                    Id = id
                });

                if (athlete == null)
                    return null;

                if (!athlete.Active)
                    return AthleteRow.NonActive;

                return athlete;
            }
        }

        public async Task<AthleteRow> GetAthleteByStravaIdAsync(string id)
        {
            using (var conn = _connectionProvider.Create())
            {
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id, FirstName, LastName, Active FROM dbo.Athletes WHERE ExternalId=@ExternalId", new
                {
                    ExternalId = id
                });

                if (athlete == null)
                    return null;

                if (!athlete.Active)
                    return AthleteRow.NonActive;

                return athlete;
            }
        }

        public async Task<AthleteRow> GetAthleteByAadIdAsync(Guid aadId)
        {
            using (var conn = _connectionProvider.Create())
            {
                AccountEntity account = await _accountsStore.GetAccountByActiveDirectoryId(aadId);
                await conn.OpenWithRetryAsync();

                var athlete = await conn.QuerySingleOrDefaultAsync<AthleteRow>("SELECT Id, FirstName, LastName, Active FROM dbo.Athletes WHERE Id=@Id", new
                {
                    account.Id
                });

                if (athlete == null)
                    return null;

                if (!athlete.Active)
                    return AthleteRow.NonActive;

                return athlete;
            }
        }
    }
}
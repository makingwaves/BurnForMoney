using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Extensions;

namespace BurnForMoney.Infrastructure.Persistence
{
    public class MemoryAccountsStore : IAccountsStore
    {
        private readonly List<AccountEntity> _accounts = new List<AccountEntity>();

        public Task<bool> TryCreateAccount(AccountEntity account)
        {
            if (_accounts.Any(x => x.ActiveDirectoryId == account.ActiveDirectoryId)) return Task.FromResult(false);
            _accounts.Add(account);
            return Task.FromResult(true);
        }

        public Task<AccountEntity> GetAccountByActiveDirectoryId(Guid activeDirectoryId)
        {
            return Task.FromResult(_accounts.SingleOrDefault(x => x.PartitionKey == activeDirectoryId.ToUpperInvariant()));
        }

        public Task<AccountEntity> GetAccountById(Guid accountId)
        {
            return Task.FromResult(_accounts.SingleOrDefault(x => x.RowKey == accountId.ToUpperInvariant()));
        }

        public async Task<bool> DeleteIfExistsAsync(Guid activeDirectoryId)
        {
            AccountEntity entity = await GetAccountByActiveDirectoryId(activeDirectoryId);
            return entity != null && _accounts.Remove(entity);
        }
    }
}
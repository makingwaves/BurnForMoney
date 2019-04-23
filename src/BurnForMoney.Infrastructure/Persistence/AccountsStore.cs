using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Infrastructure.Persistence
{
    public interface IAccountsStore
    {
        Task<bool> TryCreateAccount(AccountEntity account);
        Task<AccountEntity> GetAccountByActiveDirectoryId(Guid activeDirectoryId);
        Task<AccountEntity> GetAccountById(Guid accountId);
        Task<bool> DeleteIfExistsAsync(Guid activeDirectoryId);
    }

    public class AccountsStore : IAccountsStore
    {
        private readonly CloudTable _accountsTable;

        public AccountsStore(string storageConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _accountsTable = tableClient.GetTableReference("Accounts");
        }

        public async Task<bool> TryCreateAccount(AccountEntity account)
        {
            if (await AccountExists(account.ActiveDirectoryId)) return false;
            TableOperation insert = TableOperation.Insert(account, echoContent: true);
            TableResult result = await _accountsTable.ExecuteAsync(insert);
            return result.Result is AccountEntity;
        }

        public async Task<bool> DeleteIfExistsAsync(Guid activeDirectoryId)
        {
            if (!await AccountExists(activeDirectoryId)) return false;
            AccountEntity entity = await GetAccountByActiveDirectoryId(activeDirectoryId);
            if (entity == null) return false;

            TableOperation delete = TableOperation.Delete(entity);
            TableResult result = await _accountsTable.ExecuteAsync(delete);
            return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
        }

        public async Task<AccountEntity> GetAccountByActiveDirectoryId(Guid activeDirectoryId)
        {
            TableQuery<AccountEntity> query = GuidEqualityQuery(nameof(AccountEntity.PartitionKey), activeDirectoryId);
            return await GetSingleOrDefault(query);
        }

        public async Task<AccountEntity> GetAccountById(Guid accountId)
        {
            TableQuery<AccountEntity> query = GuidEqualityQuery(nameof(AccountEntity.RowKey), accountId);
            return await GetSingleOrDefault(query);
        }

        private async Task<AccountEntity> GetSingleOrDefault(TableQuery<AccountEntity> query)
        {
            try
            {
                TableQuerySegment<AccountEntity> result = await _accountsTable.ExecuteQuerySegmentedAsync(query, null);
                return result.Results?.SingleOrDefault();
            }
            catch (StorageException ex)
            {
                throw new InvalidOperationException(ex.RequestInformation?.ExtendedErrorInformation?.ErrorMessage);
            }
        }

        private static TableQuery<AccountEntity> GuidEqualityQuery(string key, Guid value)
        {
            return new TableQuery<AccountEntity>().Where(
                TableQuery.GenerateFilterCondition(key, QueryComparisons.Equal, value.ToString("D"))
            );
        }

        private async Task<bool> AccountExists(Guid activeDirectoryId)
        {
            return await GetAccountByActiveDirectoryId(activeDirectoryId) != null;
        }
    }
}
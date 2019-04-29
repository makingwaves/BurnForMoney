using System;
using BurnForMoney.Infrastructure.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Infrastructure.Persistence
{
    public class AccountEntity : TableEntity
    {
        public Guid Id => Guid.Parse(RowKey);
        public Guid ActiveDirectoryId => Guid.Parse(PartitionKey);

        // Required by Table Storage API
        public AccountEntity()
        {
        }

        public AccountEntity(Guid id, Guid activeDirectoryId)
        {
            RowKey = id.ToUpperInvariant();
            PartitionKey = activeDirectoryId.ToUpperInvariant();
        }
    }
}
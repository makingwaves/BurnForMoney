using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Infrastructure.Persistence
{
    public class AccountEntity : TableEntity
    {
        public Guid Id { get; }
        public Guid ActiveDirectoryId { get; }

        // Required by Table Storage API
        public AccountEntity()
        {
        }

        public AccountEntity(Guid id, Guid activeDirectoryId)
        {
            Id = id;
            ActiveDirectoryId = activeDirectoryId;
            RowKey = id.ToString("D");
            PartitionKey = activeDirectoryId.ToString("D");
        }
    }
}
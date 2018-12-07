using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public class DomainEventEntity : TableEntity
    {
        public string AggregateId { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public DomainEventEntity(string partitionKey, string rowKey = null)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey ?? string.Empty;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public DomainEventEntity()
        {

        }
    }
}
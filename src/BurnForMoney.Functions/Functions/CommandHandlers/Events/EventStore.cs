using System.Threading.Tasks;
using BurnForMoney.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public interface IEventStore
    {
        Task SaveAsync(DomainEvent @event);
        Task SaveAsync(DomainEvent[] events);
    }

    public class EventStore : IEventStore
    {
        private readonly CloudTable _domainEventsTable;

        private EventStore(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _domainEventsTable = tableClient.GetTableReference("DomainEvents");
        }

        public Task SaveAsync(DomainEvent @event)
        {
            return SaveAsync(new[] { @event });
        }

        public async Task SaveAsync(DomainEvent[] events)
        {
            foreach (var domainEvent in events)
            {
                var eventEntity = new DomainEventEntity(domainEvent.SagaId)
                {
                    Name = domainEvent.Name,
                    AggregateId = domainEvent.SagaId,
                    Data = JsonConvert.SerializeObject(domainEvent),
                    ETag = "*"
                };

                var operation = TableOperation.Insert(eventEntity);
                await _domainEventsTable.ExecuteAsync(operation);
            }
        }

        public static IEventStore Create(string storageConnectionString)
        {
            return new EventStore(storageConnectionString);
        }
    }
}
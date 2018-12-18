using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Streamstone;

namespace BurnForMoney.Infrastructure
{
    public interface IEventStore
    {
        Task SaveAsync(Guid aggregateId, DomainEvent[] events, int expectedVersion);
        Task<List<DomainEvent>> GetEventsForAggregateAsync(Guid aggregateId);
    }

    public class EventStore : IEventStore
    {
        private readonly CloudTable _domainEventsTable;
        private readonly IEventPublisher _eventPublisher;

        private EventStore(string storageConnectionString, IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _domainEventsTable = tableClient.GetTableReference("DomainEvents");
        }

        public async Task SaveAsync(Guid aggregateId, DomainEvent[] events, int expectedVersion)
        {
            var i = expectedVersion;

            foreach (var @event in events)
            {
                i++;
                @event.Version = i;
            }

            var paritionKey = aggregateId.ToString("D");
            var partition = new Partition(_domainEventsTable, paritionKey);

            var existent = await Stream.TryOpenAsync(partition);
            var stream = existent.Found
                ? existent.Stream
                : new Stream(partition);

            if (stream.Version != expectedVersion)
                throw new ConcurrencyException();

            try
            {
                var eventsData = events.Select(ToEventData);
                await Stream.WriteAsync(stream, eventsData.ToArray());
            }
            catch (ConcurrencyConflictException)
            {
                throw new ConcurrencyException();
            }

            await _eventPublisher.PublishAsync(events);
        }

        private static EventData ToEventData(object e)
        {
            var id = Guid.NewGuid();

            var properties = new
            {
                Id = id,                 
                Type = e.GetType().FullName, 
                Data = Json(e)     
            };

            return new EventData(EventId.From(id), EventProperties.From(properties));
        }

        private static string Json(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public async Task<List<DomainEvent>> GetEventsForAggregateAsync(Guid aggregateId)
        {
            var paritionKey = aggregateId.ToString("D");
            var partition = new Partition(_domainEventsTable, paritionKey);

            if (!await Stream.ExistsAsync(partition))
            {
                throw new AggregateNotFoundException();
            }

            var partitionRead = await Stream.ReadAsync<DomainEventEntity>(partition);
            return partitionRead.Events.Select(ToEvent).ToList();
        }

        private static DomainEvent ToEvent(DomainEventEntity e)
        {
            return (DomainEvent)JsonConvert.DeserializeObject(e.Data, Type.GetType(e.Type));
        }

        public static IEventStore Create(string storageConnectionString, IEventPublisher eventPublisher)
        {
            return new EventStore(storageConnectionString, eventPublisher);
        }
    }

    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;

namespace BurnForMoney.Infrastructure.Persistence
{
    public class MemoryEventStore : IEventStore
    {
        public Dictionary<Guid, List<DomainEvent>> _domainsEvents = new Dictionary<Guid, List<DomainEvent>>();

        public Task<List<DomainEvent>> GetEventsForAggregateAsync(Guid aggregateId)
        {
            return Task.FromResult(_domainsEvents[aggregateId]);
        }

        public Task SaveAsync(Guid aggregateId, DomainEvent[] events, int expectedVersion)
        {
            ensureEventStoreForAggregate(aggregateId);
            updateEventsVersions(events.ToList(), expectedVersion);
            checkForConcurencyException(aggregateId, expectedVersion);
            
            _domainsEvents[aggregateId].AddRange(events);

            return Task.CompletedTask;
        }

        private void checkForConcurencyException(Guid aggregateId, int expectedVersion)
        {
            if(_domainsEvents[aggregateId].Any() 
            && _domainsEvents[aggregateId].Last().Version != expectedVersion)
                throw new ConcurrencyException();
        }

        private void updateEventsVersions(List<DomainEvent> events, int expectedVersion) 
            => events.ForEach(e => e.Version = ++expectedVersion);

        private void ensureEventStoreForAggregate(Guid aggregateId)
        {
            if(!_domainsEvents.ContainsKey(aggregateId))
                _domainsEvents.Add(aggregateId, new List<DomainEvent>());
        }
    }
}
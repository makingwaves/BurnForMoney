using System;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain.Domain;

namespace BurnForMoney.Domain
{
    public interface IRepository<T> where T : IAggregateRoot, new()
    {
        Task SaveAsync(IAggregateRoot aggregate, int expectedVersion);
        Task<T> GetByIdAsync(Guid id);
    }

    public class Repository<T> : IRepository<T> where T : IAggregateRoot, new()
    {
        private readonly IEventStore _storage;

        public Repository(IEventStore storage)
        {
            _storage = storage;
        }

        public async Task SaveAsync(IAggregateRoot aggregate, int expectedVersion)
        {
            await _storage.SaveAsync(aggregate.Id, aggregate.GetUncommittedEvents().ToArray(), expectedVersion);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var obj = new T();
            var e = await _storage.GetEventsForAggregateAsync(id);
            obj.LoadsFromHistory(e);
            return obj;
        }
    }
}
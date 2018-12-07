using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace BurnForMoney.Infrastructure
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(IEnumerable<T> events) where T : DomainEvent;
    }

    public class EventsDispatcher : IEventPublisher
    {   
        private readonly EventGridClient _eventGridClient;
        private readonly string _topicHostname;

        public EventsDispatcher(string sasKey, string topicEndpoint)
        {
            var topicCredentials = new TopicCredentials(sasKey);
            _eventGridClient = new EventGridClient(topicCredentials);
            _topicHostname = new Uri(topicEndpoint).Host;
        }

        public async Task PublishAsync<T>(IEnumerable<T> events) where T: DomainEvent
        {
            var eventsList = GetEventsList(events);

            await _eventGridClient.PublishEventsAsync(_topicHostname, eventsList);
        }

        static IList<EventGridEvent> GetEventsList(IEnumerable<DomainEvent> @events)
        {
            var eventsList = new List<EventGridEvent>();

            foreach (var domainEvent in events)
            {
                eventsList.Add(new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = domainEvent.GetType().FullName,
                    EventTime = DateTime.UtcNow,
                    Subject = domainEvent.GetType().Name,
                    Data = domainEvent,
                    DataVersion = domainEvent.Version.ToString()
                });
            }

            return eventsList;
        }
    }
}
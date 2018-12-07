using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Shared.Events
{
    public class EventsDispatcher
    {   
        private readonly EventGridClient _eventGridClient;
        private readonly string _topicHostname;

        public EventsDispatcher(string sasKey, string topicEndpoint)
        {
            var topicCredentials = new TopicCredentials(sasKey);
            _eventGridClient = new EventGridClient(topicCredentials);
            _topicHostname = new Uri(topicEndpoint).Host;
        }

        public async Task DispatchAsync<T>(IEnumerable<T> events) where T: DomainEvent
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
                    EventType = domainEvent.Name,
                    EventTime = domainEvent.TimeStamp,
                    Subject = domainEvent.Name,
                    Data = domainEvent,
                    DataVersion = domainEvent.Version
                });
            }

            return eventsList;
        }
    }
}
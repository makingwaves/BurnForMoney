using System;
using BurnForMoney.Domain;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BurnForMoney.Functions.Shared.Extensions
{
    public static class EventSubscriptionExtensions
    {
        public static void LogEventAcquisition(this ILogger log, EventGridEvent @event)
        {
            log.LogInformation("-------Event data received-------\n");
            log.LogInformation($"Event => {@event.EventType} Subject => {@event.Subject}\n");
        }

        public static DomainEvent AssertDomainMembership(this EventGridEvent @event)
        {
            if (!(@event.Data is JObject eventData))
            {
                throw new ArgumentException(nameof(@event.Data));
            }

            Type eventType = Type.GetType(@event.EventType);
            if (!(eventData.ToObject(eventType) is DomainEvent domainEvent))
            {
                throw new ArgumentException(@event.EventType);
            }

            return domainEvent;
        }
    }
}

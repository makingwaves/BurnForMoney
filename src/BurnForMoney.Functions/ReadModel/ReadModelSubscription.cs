using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure;
using BurnForMoney.Infrastructure.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BurnForMoney.Functions.ReadModel
{
    public static class ReadModelSubscription
    {
        [FunctionName("EventGrid_ReadModelSubscription")]
        public static async Task<IActionResult> EventGrid_ReadModelSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogInformation("-------Event data received-------\n");
            log.LogInformation($"Event => {@event.EventType} Subject => {@event.Subject}\n");

            if (!(@event.Data is JObject eventData))
            {
                throw new ArgumentException(nameof(@event.Data));
            }

            var eventType = Type.GetType(@event.EventType);

            if (!(eventData.ToObject(eventType) is DomainEvent receivedEvent))
            {
                throw new ArgumentException(@event.EventType);
            }

            var handler = new ViewFactory(configuration).GetFor(receivedEvent);

            await handler.HandleAsync(receivedEvent);

            return new OkResult();
        }
    }

    public class ViewFactory
    {
        private readonly ConfigurationRoot _configuration;

        public ViewFactory(ConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public IHandles<T> GetFor<T>(T domainEvent) where T: DomainEvent
        {
            IHandles<T> handler = null;
            if (domainEvent is AthleteCreated)
            {
                handler = (IHandles<T>)new AthleteView(_configuration.ConnectionStrings.SqlDbConnectionString);
            }
            else if (domainEvent is ActivityAdded || domainEvent is ActivityUpdated || domainEvent is ActivityDeleted)
            {
                handler = (IHandles<T>)new ActivityView(_configuration.ConnectionStrings.SqlDbConnectionString);
            }

            return handler;
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Events;
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
        public static async Task EventGrid_ReadModelSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogInformation("-------Event data reviewed-------\n");
            log.LogInformation($"Event => {@event.EventType} Subject => {@event.Subject}\n");

            if (!(@event.Data is JObject eventData))
            {
                throw new ArgumentException(nameof(@event.Data));
            }

            var eventType = Type.GetType(@event.EventType);
            var receivedEvent = eventData.ToObject(eventType);
            if (receivedEvent is AthleteCreated created)
            {
                var handler = new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString);
                await handler.HandleAsync(created);
                return;
            }

            if (receivedEvent is ActivityAdded activityAdded)
            {
                var handler = new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString);
                await handler.HandleAsync(activityAdded);
                return;
            }
        }
    }
}
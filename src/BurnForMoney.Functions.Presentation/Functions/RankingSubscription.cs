using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Presentation.Configuration;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BurnForMoney.Functions.Presentation.Functions
{
    public static class RankingSubscription
    {      
        [FunctionName("EventGrid_RankingSubscription")]
        public static async Task EventGrid_RankingSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
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

            switch (receivedEvent)
            {
                case ActivityAdded activityAdded:
                    await new RankingView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityAdded);
                    break;
                case ActivityUpdated_V2 activityUpdated:
                    await new RankingView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityUpdated);
                    break;
                case ActivityDeleted_V2 activityDeleted:
                    await new RankingView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityDeleted);
                    break;
                default:
                    break;
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.ReadModel.Configuration;
using BurnForMoney.ReadModel.Views;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BurnForMoney.ReadModel
{
    public static class ReadModelSubscription
    {
        [FunctionName("EventGrid_ReadModelSubscription")]
        public static async Task EventGrid_ReadModelSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
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
                case AthleteCreated created:
                    await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(created);
                    break;
                case AthleteDeactivated deactivated:
                    await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(deactivated);
                    break;
                case AthleteActivated activated:
                    await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activated);
                    break;
                case ActivityAdded activityAdded:
                    await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityAdded);
                    break;
                case ActivityUpdated activityUpdated:
                    await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityUpdated);
                    break;
                case ActivityDeleted activityDeleted:
                    await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(activityDeleted);
                    break;
                case PointsGranted pointsGranted:
                case PointsLost pointsLost:
                    break;
                default:
                    throw new NotSupportedException($"Event type: {receivedEvent.GetType()} is not supported.");
            }
        }
    }
}
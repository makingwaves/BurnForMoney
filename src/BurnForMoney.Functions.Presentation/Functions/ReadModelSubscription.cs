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

            try
            {
                switch (receivedEvent)
                {
                    case AthleteCreated created:
                        await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(
                            created);
                        break;
                    case ActiveDirectoryIdAssigned activeDirectoryIdAssigned:
                        await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(
                            activeDirectoryIdAssigned);
                        break;
                    case AthleteDeactivated deactivated:
                        await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(
                            deactivated);
                        break;
                    case AthleteActivated activated:
                        await new AthleteView(configuration.ConnectionStrings.SqlDbConnectionString).HandleAsync(
                            activated);
                        break;
                    case ActivityAdded activityAdded:
                        await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString, log).HandleAsync(
                            activityAdded);
                        break;
                    case ActivityUpdated_V2 activityUpdated:
                        await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString, log).HandleAsync(
                            activityUpdated);
                        break;
                    case ActivityDeleted_V2 activityDeleted:
                        await new ActivityView(configuration.ConnectionStrings.SqlDbConnectionString, log).HandleAsync(
                            activityDeleted);
                        break;
                    default:
                        throw new NotSupportedException($"Event type: {receivedEvent.GetType()} is not supported.");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"{ex.Message}");
                throw;
            }
        }
    }
}
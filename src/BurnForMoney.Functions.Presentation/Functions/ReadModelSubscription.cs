using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Presentation.Configuration;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Presentation.Functions
{
    public static class ReadModelSubscription
    {
        [FunctionName("EventGrid_ReadModelSubscription")]
        public static async Task EventGrid_ReadModelSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogEventAcquisition(@event);
            DomainEvent receivedEvent = @event.AssertDomainMembership();

            var dispatcher = new PresentationEventsDispatcher(configuration.ConnectionStrings.SqlDbConnectionString, log);
            await dispatcher.DispatchAthleteEvent(receivedEvent);
        }
    }
}
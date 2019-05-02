using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Presentation.Functions
{
    public static class RankingSubscription
    {
        [FunctionName("EventGrid_RankingSubscription")]
        public static async Task EventGrid_RankingSubscription([EventGridTrigger] EventGridEvent @event, ILogger log,
            [Inject] IPresentationEventsDispatcherFactory dispatcherFactory)
        {
            log.LogEventAcquisition(@event);
            DomainEvent receivedEvent = @event.AssertDomainMembership();

            IPresentationEventsDispatcher dispatcher = dispatcherFactory.Create(log);
            await dispatcher.DispatchActivityEvent(receivedEvent);
        }
    }
}
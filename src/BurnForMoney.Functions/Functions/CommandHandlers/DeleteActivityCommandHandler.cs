using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.CommandHandlers.Events;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class DeleteActivityCommandHandler
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] DeleteActivityCommand deleteActivityCommand,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeleteActivity);

            var eventStore = EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage,
                new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint));

            var @event = new ActivityDeleted
            {
                ActivityId = deleteActivityCommand.Id
            };

            await eventStore.SaveAsync(deleteActivityCommand.AthleteId, new DomainEvent[] { @event }, @event.Version);
        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Events;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AddActivityCommandHandler
    {
        [FunctionName(FunctionsNames.Q_AddActivity)]
        public static async Task Q_AddActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.AddActivityRequests)] AddActivityCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_AddActivity);

            var eventStore = EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage,
                new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint));

            var @event = new ActivityAdded(message.Id, message.AthleteId, message.ExternalId, message.DistanceInMeters,
                message.MovingTimeInMinutes, message.ActivityType, message.StartDate, message.Source);

            await eventStore.SaveAsync(message.AthleteId, new DomainEvent[] { @event }, @event.Version);
            log.LogInformation(nameof(FunctionsNames.Q_AddActivity), "Logged event.");
            
            log.LogFunctionEnd(FunctionsNames.Q_AddActivity);
        }
    }
}
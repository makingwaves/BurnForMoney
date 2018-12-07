using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.CommandHandlers.Events;
using BurnForMoney.Functions.Shared.Commands;
using BurnForMoney.Functions.Shared.Events;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AddActivityCommandHandler
    {
        [FunctionName(FunctionsNames.Q_AddActivity)]
        public static async Task Q_AddActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.AddActivityRequests)] AddActivityCommand addActivityCommand,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_AddActivity);

            var eventStore = EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage);

            var @event = new ActivityAdded
            {
                ActivityId = addActivityCommand.Id,
                DistanceInMeters = addActivityCommand.DistanceInMeters,
                MovingTimeInMinutes = addActivityCommand.MovingTimeInMinutes,
                ExternalId = addActivityCommand.ExternalId,
                ActivityType = addActivityCommand.ActivityType,
                StartDate = addActivityCommand.StartDate,
                Source = addActivityCommand.Source,
                SagaId = addActivityCommand.AthleteId
            };

            await eventStore.SaveAsync(@event);
            log.LogInformation(nameof(FunctionsNames.Q_AddActivity), $"Logged event: {@event.Name}");

            var eventsDispatcher = new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint);
            await eventsDispatcher.DispatchAsync(new[] { @event }); //TODO: Compensation

            log.LogInformation(nameof(FunctionsNames.Q_AddActivity), $"Dispatched event: {@event.Name}");

            log.LogFunctionEnd(FunctionsNames.Q_AddActivity);
        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.CommandHandlers.Events;
using BurnForMoney.Functions.Shared.Commands;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class UpdateActivityCommandHandler
    {
        [FunctionName(FunctionsNames.Q_UpdateActivity)]
        public static async Task ProcessUpdatedActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.UpdateActivityRequests)] UpdateActivityCommand updateActivityCommand,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_UpdateActivity);

            var eventStore = EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage);

            var @event = new ActivityUpdated
            {
                ActivityId = updateActivityCommand.Id,
                DistanceInMeters = updateActivityCommand.DistanceInMeters,
                MovingTimeInMinutes = updateActivityCommand.MovingTimeInMinutes,
                ExternalId = updateActivityCommand.ExternalId,
                ActivityType = updateActivityCommand.ActivityType,
                StartDate = updateActivityCommand.StartDate,
                Source = updateActivityCommand.Source,
                SagaId = updateActivityCommand.AthleteId
            };

            await eventStore.SaveAsync(@event);
        }
    }
}
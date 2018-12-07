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
    public class DeleteActivityCommandHandler
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] DeleteActivityCommand deleteActivityCommand,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeleteActivity);

            var eventStore = EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage);

            var @event = new ActivityDeleted
            {
                ActivityId = deleteActivityCommand.Id,
                SagaId = deleteActivityCommand.AthleteId
            };

            await eventStore.SaveAsync(@event);
        }
    }
}
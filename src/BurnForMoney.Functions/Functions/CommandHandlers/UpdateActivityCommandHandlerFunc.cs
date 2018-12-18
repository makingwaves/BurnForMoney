using System.Threading.Tasks;
using BurnForMoney.Domain.CommandHandlers;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class UpdateActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_UpdateActivity)]
        public static async Task ProcessUpdatedActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.UpdateActivityRequests)] UpdateActivityCommand message)
        {
            log.LogFunctionStart(FunctionsNames.Q_UpdateActivity);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new UpdateActivityCommandHandler(repository);
            await commandHandler.HandleAsync(message);

            log.LogFunctionEnd(FunctionsNames.Q_UpdateActivity);
        }
    }
}
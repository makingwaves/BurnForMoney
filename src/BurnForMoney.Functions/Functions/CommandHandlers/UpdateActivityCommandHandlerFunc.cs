using System.Threading.Tasks;
using BurnForMoney.Functions.Repositories;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;

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
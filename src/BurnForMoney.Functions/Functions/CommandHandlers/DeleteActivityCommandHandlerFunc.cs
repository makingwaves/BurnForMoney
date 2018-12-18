using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure.CommandHandlers;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class DeleteActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] DeleteActivityCommand message)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeleteActivity);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new DeleteActivityCommandHandler(repository);
            await commandHandler.HandleAsync(message);

            log.LogFunctionEnd(FunctionsNames.Q_DeleteActivity);
        }
    }
}
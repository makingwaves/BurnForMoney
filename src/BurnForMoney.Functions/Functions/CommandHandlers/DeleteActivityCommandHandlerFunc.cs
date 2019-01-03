using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Repositories;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class DeleteActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] DeleteActivityCommand message)
        {
            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new DeleteActivityCommandHandler(repository);
            await commandHandler.HandleAsync(message);
        }
    }
}
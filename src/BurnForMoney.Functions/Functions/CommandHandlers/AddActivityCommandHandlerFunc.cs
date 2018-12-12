using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure.CommandHandlers;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AddActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddActivity)]
        public static async Task Q_AddActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.AddActivityRequests)] AddActivityCommand message)
        {
            log.LogFunctionStart(FunctionsNames.Q_AddActivity);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new AddActivityCommandHandler(repository);
            await commandHandler.HandleAsync(message);

            log.LogFunctionEnd(FunctionsNames.Q_AddActivity);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AddActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddActivity)]
        public static async Task Q_AddActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.AddActivityRequests)] AddActivityCommand message,
            [Inject] ICommandHandler<AddActivityCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
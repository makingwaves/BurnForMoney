using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class UpdateActivityCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_UpdateActivity)]
        public static async Task ProcessUpdatedActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.UpdateActivityRequests)] UpdateActivityCommand message,
            [Inject] ICommandHandler<UpdateActivityCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AddStravaAccountCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddStravaAccount)]
        public static async Task Q_AddStravaAccountCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AddStravaAccountRequests)]
            AddStravaAccountCommand message,
            [Inject] ICommandHandler<AddStravaAccountCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
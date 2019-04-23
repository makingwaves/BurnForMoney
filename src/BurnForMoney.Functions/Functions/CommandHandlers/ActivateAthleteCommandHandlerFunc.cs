using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class ActivateAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_ActivateAthlete)]
        public static async Task Q_ActivateAthleteAsync(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.ActivateAthleteRequests)] ActivateAthleteCommand message,
            [Inject] ICommandHandler<ActivateAthleteCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
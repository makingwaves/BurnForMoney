using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class CreateAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddAthlete)]
        public static async Task Q_CreateNewAthleteCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AddAthleteRequests)] CreateAthleteCommand message,
            [Inject] ICommandHandler<CreateAthleteCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
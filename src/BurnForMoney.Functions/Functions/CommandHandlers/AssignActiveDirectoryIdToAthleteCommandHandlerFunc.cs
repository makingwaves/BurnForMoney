using System.Threading.Tasks;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class AssignActiveDirectoryIdToAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AssignActiveDirectoryIdToAthlete)]
        public static async Task Q_AssignActiveDirectoryIdToAthleteCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AssignActiveDirectoryIdRequests)] AssignActiveDirectoryIdToAthleteCommand message,
            [Inject] ICommandHandler<AssignActiveDirectoryIdToAthleteCommand> commandHandler)
        {
            await commandHandler.HandleAsync(message);
        }
    }
}
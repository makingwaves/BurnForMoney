using System.Threading.Tasks;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Repositories;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class AssignActiveDirectoryIdToAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AssignActiveDirectoryIdToAthlete)]
        public static async Task Q_AssignActiveDirectoryIdToAthleteCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AssignActiveDirectoryIdRequests)] AssignActiveDirectoryIdToAthleteCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new AssignActiveDirectoryIdToAthleteCommandHandler(repository);

            await commandHandler.HandleAsync(message);
        }
    }
}
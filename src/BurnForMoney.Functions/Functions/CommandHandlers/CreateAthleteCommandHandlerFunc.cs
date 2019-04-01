using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Repositories;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Infrastructure.Queues;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class CreateAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddAthlete)]
        public static async Task Q_CreateNewAthleteCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AddAthleteRequests)] CreateAthleteCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new CreateAthleteCommandHandler(repository);

            await commandHandler.HandleAsync(message);
        }
    }
}
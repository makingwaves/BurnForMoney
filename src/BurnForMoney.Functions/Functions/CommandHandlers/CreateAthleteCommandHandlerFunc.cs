using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure.CommandHandlers;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class CreateAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddAthlete)]
        public static async Task Q_CreateNewAthleteCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AddAthleteRequests)] CreateAthleteCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_AddAthlete);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new CreateAthleteCommandHandler(repository);
            await commandHandler.HandleAsync(message);

            log.LogFunctionEnd(FunctionsNames.Q_AddAthlete);
        }
    }
}
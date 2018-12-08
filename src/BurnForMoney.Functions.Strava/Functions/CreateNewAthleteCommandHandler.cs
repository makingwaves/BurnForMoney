using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Infrastructure.CommandHandlers;
using BurnForMoney.Infrastructure.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions
{
    public static class CreateNewAthleteCommandHandler
    {
        [FunctionName(FunctionsNames.Q_CreateNewAthleteCommandHandler)]
        public static async Task Q_CreateNewAthleteCommandHandler(ILogger log,
            [QueueTrigger(QueueNames.AddAthleteRequests)] CreateAthleteCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_CreateNewAthleteCommandHandler);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new CreateAthleteCommandHandler(repository);
            await commandHandler.HandleAsync(message);

            log.LogFunctionEnd(FunctionsNames.Q_CreateNewAthleteCommandHandler);
        }
    }
}
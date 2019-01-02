using System.Threading.Tasks;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Repositories;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public class ActivateAthleteCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_ActivateAthlete)]
        public static async Task Q_AtivateAthleteAsync(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.ActivateAthleteRequests)] ActivateAthleteCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_ActivateAthlete);

            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new ActivateAthleteCommandHandler(repository);
            await commandHandler.HandleAsync(message);
   
            log.LogFunctionEnd(FunctionsNames.Q_ActivateAthlete);
        }
    }
}
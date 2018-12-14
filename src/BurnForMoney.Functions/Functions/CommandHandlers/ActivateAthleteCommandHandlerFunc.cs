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
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
    public static class AddStravaAccountCommandHandlerFunc
    {
        [FunctionName(FunctionsNames.Q_AddStravaAccount)]
        public static async Task Q_AddStravaAccountCommandHandler(ILogger log,
            [QueueTrigger(AppQueueNames.AddStravaAccountRequests)]
            AddStravaAccountCommand message,
            [Configuration] ConfigurationRoot configuration)
        {
            var repository = AthleteRepositoryFactory.Create();
            var handler = new AddStravaAccountCommandHandler(repository);

            await handler.HandleAsync(message);
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
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
            [Configuration] ConfigurationRoot configuration,
            [Queue(StravaQueueNames.CollectAthleteActivities, Connection = "StravaQueuesStorage")] CloudQueue outputQueue)
        {
            var repository = AthleteRepositoryFactory.Create();
            var commandHandler = new CreateAthleteCommandHandler(repository);

            await commandHandler.HandleAsync(message);
            await ScheduleCollectionOfHistoricalActivitiesAsync(message.Id, outputQueue);
        }

        private static async Task ScheduleCollectionOfHistoricalActivitiesAsync(Guid athleteId, CloudQueue outputQueue)
        {
            var message = new CollectStravaActivitiesRequestMessage
            {
                AthleteId = athleteId,
                From = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
            };

            var json = JsonConvert.SerializeObject(message);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Commands;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.DeactivateAthlete
{
    public static class DeactivateAthleteFunc
    {
        [FunctionName(FunctionsNames.Q_DeactivateAthlete)]
        public static async Task Q_DeactivateAthlete([QueueTrigger(QueueNames.DeactivateAthleteRequests)] string athleteId,
            ILogger log,
            [Queue(AppQueueNames.DeactivateAthleteRequests, Connection = "AppQueuesStorage")] CloudQueue deactivateAthleteRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var id = Guid.Parse(athleteId);

            var command = new DeactivateAthleteCommand(id);
            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await deactivateAthleteRequestsQueue.AddMessageAsync(message);

            await AccessTokensStore.DeactivateAccessTokenOfAsync(id, configuration.Strava.AccessTokensKeyVaultUrl);
        }
    }
}
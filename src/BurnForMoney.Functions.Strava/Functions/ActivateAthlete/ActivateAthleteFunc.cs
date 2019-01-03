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

namespace BurnForMoney.Functions.Strava.Functions.ActivateAthlete
{
    public static class ActivateAthlete
    {
        [FunctionName(FunctionsNames.Q_ActivateAthlete)]
        public static async Task Q_ActivateAthlete([QueueTrigger(FunctionsNames.ActivateAthleteRequests)] string athleteId,
            ILogger log,
            [Queue(AppQueueNames.ActivateAthleteRequests, Connection = "AppQueuesStorage")] CloudQueue activateAthleteRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var id = Guid.Parse(athleteId);

            var command = new ActivateAthleteCommand(id);
            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await activateAthleteRequestsQueue.AddMessageAsync(message);

            await AccessTokensStore.ActivateAccessTokenOfAsync(id, configuration.Strava.AccessTokensKeyVaultUrl);
        }
    }
}
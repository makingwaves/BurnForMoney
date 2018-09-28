using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.GenerateAccessTokenFunctions
{
    public static class AuthorizeNewAthleteStarter
    {
        [FunctionName(FunctionsNames.AuthorizeNewAthleteStarter)]
        public static async Task Start([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.AuthorizeNewAthleteStarter} queue trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_AuthorizeNewAthlete, myQueueItem);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_AuthorizeNewAthlete}` with ID = `{instanceId}`.");
        }
    }
}
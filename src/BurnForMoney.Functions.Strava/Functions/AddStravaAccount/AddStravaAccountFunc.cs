using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Commands;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Auth;
using BurnForMoney.Functions.Strava.Security;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Strava.Functions.AddStravaAccount
{
    public static class AddStravaAccountFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.H_AddStravaAccount)]
        public static async Task<IActionResult> AddStravaAccountAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "account/{athleteId:guid}")]
            HttpRequest req, ILogger log, string athleteId,
            [Queue(AppQueueNames.AddStravaAccountRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue,
            [Queue(StravaQueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues,
            [Configuration] ConfigurationRoot configuration, [Inject] IAthleteReadRepository athleteReadRepository)
        {
            var athleteIdGuid = Guid.Parse(athleteId);
            string authCode = req.Query["authCode"]; 
            var existingAthlete = await athleteReadRepository.GetAthleteByIdAsync(athleteIdGuid);

            var tokenExchangeResult = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authCode);
            await EnsureThatStravaAccountIsNotAlreadyRegistered(tokenExchangeResult.Athlete.Id, athleteReadRepository);
            
            await AssignStravaAccountToAthlete(existingAthlete.Id, tokenExchangeResult, outputQueue, configuration);
            await PullStravaActivities(existingAthlete.Id, collectActivitiesQueues);

            return new OkObjectResult(configuration.Strava.ClientId);
        }

        private static async Task EnsureThatStravaAccountIsNotAlreadyRegistered(int stravaId, IAthleteReadRepository repository)
        {
            if(await repository.AthleteWithStravaIdExistsAsync(stravaId.ToString()))
                throw new StravaAccountExistsException(stravaId.ToString());
        }

        private static async Task AssignStravaAccountToAthlete(Guid athleteId, TokenExchangeResult response, CloudQueue queue, ConfigurationRoot configuration)
        {
            await AccessTokensStore.AddAsync(athleteId, response.AccessToken, response.RefreshToken, response.ExpiresAt, configuration.Strava.AccessTokensKeyVaultUrl);

            var cmd = new AddStravaAccountCommand
            {
                AthleteId = athleteId,
                StravaId = response.Athlete.Id.ToString()
            };

            var output = JsonConvert.SerializeObject(cmd);
            await queue.AddMessageAsync(new CloudQueueMessage(output));
        }

        private static async Task PullStravaActivities(Guid athleteId, CloudQueue queue)
        {
            var input = new CollectStravaActivitiesRequestMessage
            {
                AthleteId = athleteId,
                From = null
            };

            var json = JsonConvert.SerializeObject(input);
            await queue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }
}
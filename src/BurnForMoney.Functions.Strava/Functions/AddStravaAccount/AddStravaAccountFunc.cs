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
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

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
            [Configuration] ConfigurationRoot configuration)
        {
            var athleteIdGuid = Guid.Parse(athleteId);
            string authCode = req.Query["authCode"]; 
            var athleteReadRepository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var existingAthlete = await athleteReadRepository.GetAthleteByIdAsync(athleteIdGuid);

            var tokenexceResult = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authCode);
            await EnsureThatStravaAccountIsNotAlreadyRegistred(tokenexceResult.Athlete.Id, athleteReadRepository);
            
            await AssignStravaAccountToAthelte(existingAthlete.Id, tokenexceResult, outputQueue, configuration);
            await PullStravaActivities(existingAthlete.Id, collectActivitiesQueues);

            return new OkResult();
        }

        private static async Task EnsureThatStravaAccountIsNotAlreadyRegistred(int stravaId, AthleteReadRepository repository)
        {
            if(await repository.AthleteWithStravaIdExistsAsync(stravaId.ToString()))
                throw new StravaAccountExistsException(stravaId.ToString());
        }

        private static async Task AssignStravaAccountToAthelte(Guid athleteId, TokenExchangeResult response, CloudQueue queue, ConfigurationRoot configuration)
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
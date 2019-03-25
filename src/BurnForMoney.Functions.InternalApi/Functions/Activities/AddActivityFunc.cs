using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Functions.Activities.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Identity;
using BurnForMoney.Infrastructure.Authorization;
using BurnForMoney.Infrastructure.Authorization.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Activities
{
    public static class AddActivityFunc
    {
        [FunctionName(FunctionsNames.AddActivity)]
        public static async Task<IActionResult> AddActivityAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/{athleteId:guid}/activities")] HttpRequest req, 
            ILogger log, string athleteId,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue,
            [BfmAuthorize] BfmPrincipal principal)
        {
            var athleteIdGuid = Guid.Parse(athleteId);

            if (!principal.IsAuthenticated)
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            
            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athlete = await repository.GetAthleteByAadIdAsync(principal.AadId);

            if(!athlete.IsValid() || athlete.Id != athleteIdGuid)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            
            var requestData = await req.ReadAsStringAsync();
            
            ActivityAddOrUpdateRequest model;
            try
            {
                model = JsonConvert.DeserializeObject<ActivityAddOrUpdateRequest>(requestData);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to deserialize data. {ex.Message}");
            }

            try
            {
                model.Validate();
            }
            catch (Exception ex)
            {
                log.LogError(FunctionsNames.AddActivity, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            var addActivityCommand = new AddActivityCommand
            {
                Id = ActivityIdentity.Next(),
                AthleteId = athleteIdGuid,
                ActivityType = model.Type,
                // ReSharper disable once PossibleInvalidOperationException
                StartDate = model.StartDate.Value,
                DistanceInMeters = model.DistanceInMeters ?? 0,
                MovingTimeInMinutes = model.MovingTimeInMinutes,
                Source = Source.None
            };

            var output = JsonConvert.SerializeObject(addActivityCommand);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            return new OkObjectResult(addActivityCommand.Id);
        }
    }
}
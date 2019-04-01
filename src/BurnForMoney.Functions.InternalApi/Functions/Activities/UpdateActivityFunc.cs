using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Functions.Activities.Dto;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Activities
{
    public static class UpdateActivityFunc
    {
        [FunctionName(FunctionsNames.UpdateActivity)]
        public static async Task<IActionResult> Async([HttpTrigger(AuthorizationLevel.Function, "put", Route = "athlete/{athleteId:guid}/activities/{activityId:guid}")] HttpRequest req, ExecutionContext executionContext,
            string athleteId, string activityId,
            ILogger log, [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.UpdateActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            var athleteIdGuid = Guid.Parse(athleteId);
            var activityIdGuid = Guid.Parse(activityId);
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
                log.LogError(FunctionsNames.UpdateActivity, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            var command = new UpdateActivityCommand
            {
                Id = activityIdGuid,
                AthleteId = athleteIdGuid,
                ActivityType = model.Type,
                // ReSharper disable once PossibleInvalidOperationException
                StartDate = model.StartDate.Value,
                DistanceInMeters = model.DistanceInMeters ?? 0,
                MovingTimeInMinutes = model.MovingTimeInMinutes,
            };

            var output = JsonConvert.SerializeObject(command);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            return new OkObjectResult(command.Id);
        }
    }
}
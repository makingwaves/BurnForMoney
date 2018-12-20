using System;
using System.Threading.Tasks;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
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
        public static async Task<IActionResult> Async([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "athlete/{athleteId:guid}/activities/{activityId:guid}")] HttpRequest req, ExecutionContext executionContext,
            string athleteId, string activityId,
            ILogger log,
            [Queue(AppQueueNames.UpdateActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            log.LogFunctionStart(FunctionsNames.UpdateActivity);

            var requestData = await req.ReadAsStringAsync();

            UpdateActivityRequest model;
            try
            {
                model = JsonConvert.DeserializeObject<UpdateActivityRequest>(requestData);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to deserialize data. {ex.Message}");
            }

            try
            {
                ValidateRequest(model);
            }
            catch (Exception ex)
            {
                log.LogError(FunctionsNames.UpdateActivity, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            var command = new UpdateActivityCommand
            {
                Id = Guid.Parse(activityId),
                AthleteId = Guid.Parse(athleteId),
                ActivityType = model.Type,
                // ReSharper disable once PossibleInvalidOperationException
                StartDate = model.StartDate.Value,
                DistanceInMeters = model.DistanceInMeters,
                MovingTimeInMinutes = model.MovingTimeInMinutes,
            };

            var output = JsonConvert.SerializeObject(command);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            log.LogFunctionEnd(FunctionsNames.UpdateActivity);
            return new OkObjectResult(command.Id);
        }

        private static void ValidateRequest(UpdateActivityRequest request)
        {
            if (request.StartDate == null)
            {
                throw new ArgumentNullException(nameof(request.StartDate));
            }
            if (string.IsNullOrWhiteSpace(request.Type))
            {
                throw new ArgumentNullException(nameof(request.Type));
            }
            if (request.MovingTimeInMinutes <= 0)
            {
                throw new ArgumentNullException(nameof(request.MovingTimeInMinutes));
            }
        }

        public class UpdateActivityRequest
        {
            public DateTime? StartDate { get; set; }
            public string Type { get; set; }
            public double DistanceInMeters { get; set; }
            public double MovingTimeInMinutes { get; set; }
        }
    }
}
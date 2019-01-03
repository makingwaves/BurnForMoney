using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Identity;
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
        public static async Task<IActionResult> AddActivityAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/{athleteId:guid}/activities")] HttpRequest req, ExecutionContext executionContext,
            ILogger log,
            string athleteId,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            var requestData = await req.ReadAsStringAsync();

            AddActivityRequest model;
            try
            {
                model = JsonConvert.DeserializeObject<AddActivityRequest>(requestData);
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
                log.LogError(FunctionsNames.AddActivity, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            var addActivityCommand = new AddActivityCommand
            {
                Id = ActivityIdentity.Next(),
                AthleteId = Guid.Parse(athleteId),
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

        private static void ValidateRequest(AddActivityRequest request)
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
    }

    public class AddActivityRequest
    {
        public DateTime? StartDate { get; set; }
        public string Type { get; set; }
        public double? DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
    }
}
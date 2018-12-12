using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Infrastructure.Commands;
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
        public static async Task<IActionResult> AddActivityAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/{athleteId:length(36)}/activities")] HttpRequest req, ExecutionContext executionContext,
            ILogger log,
            Guid athleteId,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            log.LogFunctionStart(FunctionsNames.AddActivity);

            var requestData = await req.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AddActivityRequest>(requestData);
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
                AthleteId = athleteId,
                ActivityType = model.Category,
                StartDate = model.StartDate.Value,
                DistanceInMeters = model.DistanceInMeters ?? 0,
                MovingTimeInMinutes = model.MovingTimeInMinutes,
                Source = "Manual"
            };

            var output = JsonConvert.SerializeObject(addActivityCommand);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            log.LogFunctionEnd(FunctionsNames.AddActivity);
            return new OkObjectResult(addActivityCommand.Id);
        }

        private static void ValidateRequest(AddActivityRequest request)
        {
            if (request.StartDate == null)
            {
                throw new ArgumentNullException(nameof(request.StartDate));
            }
            if (string.IsNullOrWhiteSpace(request.Category))
            {
                throw new ArgumentNullException(nameof(request.Category));
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
        public string Category { get; set; }
        public double? DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
    }
}
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
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
    public static class DeleteActivityFunc
    {
        [FunctionName(FunctionsNames.DeleteActivity)]
        public static async Task<IActionResult> DeleteActivity([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "athlete/{athleteId:guid}/activities/{activityId:guid}")] HttpRequest req,
            ExecutionContext executionContext, string athleteId, string activityId,
            ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue,
            [BfmAuthorize] BfmPrincipal principal)
        {
            var athleteIdGuid = Guid.Parse(athleteId);
            var activityIdGuid = Guid.Parse(activityId);
            
            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athlete = await repository.GetAthleteByAadIdAsync(principal.AadId);

            if (!athlete.IsValid() || athlete.Id != athleteIdGuid)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = activityIdGuid, AthleteId = athleteIdGuid });
            await outputQueue.AddMessageAsync(new CloudQueueMessage(json));
            return new OkObjectResult("Request received.");
        }
    }
}
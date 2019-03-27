using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.InternalApi.Functions.Athletes.Dto;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class SignInAthleteFunc
    {
        [FunctionName(FunctionsNames.SignInAthlete)]
        public static async Task<IActionResult> CreateAthleteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/signin")] HttpRequest req, 
            ExecutionContext executionContext, ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.AddAthleteRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            string requestData = await req.ReadAsStringAsync();
            CreateAthleteRequest model;

            try
            {
                model = JsonConvert.DeserializeObject<CreateAthleteRequest>(requestData);
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
                log.LogError(FunctionsNames.SignInAthlete, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            var createAthleteCommand = new CreateAthleteCommand
            {
                Id = Guid.NewGuid(),
                AadId = model.AadId,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var output = JsonConvert.SerializeObject(createAthleteCommand);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            return new OkObjectResult(createAthleteCommand.Id);
        }
    }
}
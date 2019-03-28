using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Commands;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.InternalApi.Functions.Activities.Dto;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class AsignActiveDirectoryIdToAthleteFunc
    {
        [FunctionName(FunctionsNames.AssignActiveDirectoryIdToAthlete)]
        public static async Task<IActionResult> AssignActiveDirectoryIdToAthleteAsync(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "athlete/asign_aad")] HttpRequest req,
            ExecutionContext executionContext, ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.AssignActiveDirectoryIdRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            
            var requestData = await req.ReadAsStringAsync();

            AsignActiveDirectoryIdToAthleteRequest model;
            try
            {
                model = JsonConvert.DeserializeObject<AsignActiveDirectoryIdToAthleteRequest>(requestData);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to deserialize data. {ex.Message}");
            }

            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var existingAthlete = await repository.GetAthleteByAadIdAsync(model.AadId);

            if(existingAthlete != null)
                return new BadRequestObjectResult($"Athlete with given AadId already exists.");

            var cmd = new AssignActiveDirectoryIdToAthleteCommand(model.AthleteId, model.AadId);
            var output = JsonConvert.SerializeObject(cmd);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));

            return new OkResult();
        }
    }
}
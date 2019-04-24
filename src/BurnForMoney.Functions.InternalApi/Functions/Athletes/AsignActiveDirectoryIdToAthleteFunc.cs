using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.InternalApi.Functions.Activities.Dto;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class AsignActiveDirectoryIdToAthleteFunc
    {
        [FunctionName(FunctionsNames.AssignActiveDirectoryIdToAthlete)]
        public static async Task<IActionResult> AssignActiveDirectoryIdToAthleteAsync(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "athlete/asign_aad")] HttpRequest req,
            ExecutionContext executionContext, ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Inject] IAthleteReadRepository repository,
            [Inject] IAccountsStore accountsStore,
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

            var existingAthlete = await repository.GetAthleteByIdAsync(model.AthleteId);

            if(existingAthlete == null)
                return new BadRequestObjectResult($"No Athlete with given Id found");

            if (!await accountsStore.TryCreateAccount(new AccountEntity(model.AthleteId, model.AadId)))
            {
                return new BadRequestObjectResult("Account already exists");
            }

            return new OkResult();
        }
    }
}
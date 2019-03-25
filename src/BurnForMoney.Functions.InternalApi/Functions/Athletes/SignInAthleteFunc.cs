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

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class SignInAthleteFunc
    {
        [FunctionName(FunctionsNames.SignInAthlete)]
        public static async Task<IActionResult> CreateAthleteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/signin")] HttpRequest req, 
            ExecutionContext executionContext, 
            [BfmAuthorize] BfmPrincipal principal, ILogger log,
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.AddAthleteRequests, Connection = "AppQueuesStorage")] CloudQueue outputQueue)
        {
            if (!principal.IsAuthenticated)
                return new StatusCodeResult(StatusCodes.Status403Forbidden);

            var repository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athlete = await repository.GetAthleteByAadIdAsync(principal.AadId);
            
            if (athlete != null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var createAthelteCommand = new CreateAthleteCommand
            {
                Id = Guid.NewGuid(),
                AadId = principal.AadId,
                FirstName = principal.FirstName,
                LastName = principal.FirstName
            };

            var output = JsonConvert.SerializeObject(createAthelteCommand);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            return new OkObjectResult(createAthelteCommand.Id);
        }
    }
}
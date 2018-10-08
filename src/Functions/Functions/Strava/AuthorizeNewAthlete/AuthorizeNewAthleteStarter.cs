using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteStarter
    {
        [FunctionName(FunctionsNames.AuthorizeNewAthleteStarter)]
        public static async Task Start([QueueTrigger(QueueNames.AuthorizationCodes)]string myQueueItem, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.AuthorizeNewAthleteStarter} queue trigger processed a request at {DateTime.UtcNow}.");

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_AuthorizeNewAthlete, myQueueItem);
            log.LogInformation($"Started orchestration function: `{FunctionsNames.O_AuthorizeNewAthlete}` with ID = `{instanceId}`.");
        }

        [FunctionName(FunctionsNames.SubmitAthleteApproval)]
        public static async Task<IActionResult> SubmitAthleteApprovalAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SubmitAthleteApproval/{code}")] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient client, 
            [Table("AthleteApprovals", "AthleteApproval", "{code}", Connection = "AzureWebJobsStorage")] AthleteApproval approval,
            ILogger log, string code)
        {
            log.LogInformation($"{FunctionsNames.SubmitAthleteApproval} HTTP trigger processed a request.");

            string result = req.Query["result"];
            if (string.IsNullOrWhiteSpace(result))
            {
                return new BadRequestObjectResult("Athlete approval result is required.");
            }
            var orchestrationStatus = await client.GetStatusAsync(approval.OrchestrationId);
            switch (orchestrationStatus.RuntimeStatus)
            {
                case OrchestrationRuntimeStatus.Completed:
                    return new OkObjectResult("Athlete approval request is already completed. The status cannot be updated twice.");
                case OrchestrationRuntimeStatus.Failed:
                    return new BadRequestObjectResult("Athlete approval request failed. The instance failed with an error.");
                case OrchestrationRuntimeStatus.Canceled:
                    return new BadRequestObjectResult("Athlete approval request is cancelled.");
                case OrchestrationRuntimeStatus.Terminated:
                    return new BadRequestObjectResult("Athlete approval request is teminated. The instance was abruptly terminated.");
            }

            log.LogInformation($"Sending athlete approval result to {approval.OrchestrationId} of {result}.");
            await client.RaiseEventAsync(approval.OrchestrationId, "AthleteApproval", result);

            return new OkObjectResult($"Thank you! Athlete has been {result.ToLowerInvariant()}.");
        }
    }
}
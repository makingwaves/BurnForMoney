using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteStarter
    {
        [FunctionName(FunctionsNames.AuthorizeNewAthleteStarter)]
        public static async Task Start([QueueTrigger(QueueNames.AuthorizationCodes)]string authorizationCode, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart(FunctionsNames.AuthorizeNewAthleteStarter);

            var instanceId = await starter.StartNewAsync(FunctionsNames.O_AuthorizeNewAthlete, authorizationCode);
            log.LogInformation(FunctionsNames.AuthorizeNewAthleteStarter, $"Started orchestration function: `{FunctionsNames.O_AuthorizeNewAthlete}` with ID = `{instanceId}`.");
            log.LogFunctionEnd(FunctionsNames.AuthorizeNewAthleteStarter);
        }

        [FunctionName(FunctionsNames.SubmitAthleteApproval)]
        public static async Task<IActionResult> SubmitAthleteApprovalAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SubmitAthleteApproval/{code}")] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient client, 
            [Table("AthleteApprovals", "AthleteApproval", "{code}", Connection = "AzureWebJobsStorage")] AthleteApproval approval,
            ILogger log, string code)
        {
            log.LogFunctionStart(FunctionsNames.SubmitAthleteApproval);
            
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

            log.LogInformation(FunctionsNames.SubmitAthleteApproval, $"Sending athlete approval result to {approval.OrchestrationId} of {result}.");
            await client.RaiseEventAsync(approval.OrchestrationId, "AthleteApproval", result);
            
            log.LogFunctionEnd(FunctionsNames.SubmitAthleteApproval);
            return new OkObjectResult($"Thank you! Athlete has been {result.ToLowerInvariant()}.");
        }
    }
}
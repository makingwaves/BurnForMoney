using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.Functions.Strava.Exceptions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteOrchestrator
    {
        [FunctionName(FunctionsNames.O_AuthorizeNewAthlete)]
        public static async Task O_AuthorizeNewAthlete(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_AuthorizeNewAthlete}` received a request.");
            }

            var authorizationCode = context.GetInput<string>();

            try
            {
                // 1. Exchange and authorize athlete
                var athlete = await context.CallActivityWithRetryAsync<NewStravaAthlete>(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary,
                    new RetryOptions(TimeSpan.FromSeconds(5), 3), authorizationCode);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] exchanged token for user {athlete.FirstName} " +
                                       $"{athlete.LastName}.");
                }

                // 2. Send approval request
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] sending approval email...");
                }
                await context.CallActivityAsync(FunctionsNames.A_SendAthleteApprovalRequest, (athlete.FirstName, athlete.LastName));
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] sent approval email.");
                }

                // 3. Wait for approval
                string approvalResult;
                using (var cts = new CancellationTokenSource())
                {
                    // Durable timers cannot last longer than 7 days due to limitations in Azure Storage.
                    var timeoutTaskDuration = context.CurrentUtcDateTime.AddDays(6);
                    var timeoutTask = context.CreateTimer(timeoutTaskDuration, cts.Token);
                    var approvalTask = context.WaitForExternalEvent<string>("AthleteApproval");

                    var winner = await Task.WhenAny(timeoutTask, approvalTask);
                    if (winner == approvalTask)
                    {
                        approvalResult = approvalTask.Result;
                        cts.Cancel();
                    }
                    else
                    {
                        throw new FailedToApproveAthleteTimeoutException(athlete.AthleteId, timeoutTaskDuration);
                    }
                }

                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] Athlete: {athlete.FirstName} {athlete.LastName} has been {approvalResult}.");
                }

                // 4. Process a new athlete request
                await context.CallActivityAsync(FunctionsNames.A_ProcessNewAthleteRequest, athlete);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] processed athlete's data.");
                }

            }
            catch (Exception ex)
            {
                var errorMessage = $"[{FunctionsNames.O_AuthorizeNewAthlete}] failed to authorize a new athlete in the system. {ex}";
                log.LogError(errorMessage);
                await context.CallActivityAsync(FunctionsNames.A_AuthorizeNewAthleteCompensation, new AuthorizeNewAthleteCompensation { AuthorizationCode = authorizationCode, ErrorMessage = errorMessage });
            }
        }
    }

    public class AuthorizeNewAthleteCompensation
    {
        public string AuthorizationCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
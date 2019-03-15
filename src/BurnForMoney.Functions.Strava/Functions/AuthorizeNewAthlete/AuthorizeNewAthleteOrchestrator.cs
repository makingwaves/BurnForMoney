using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteOrchestrator
    {
        [FunctionName(FunctionsNames.O_AuthorizeNewAthlete)]
        public static async Task O_AuthorizeNewAthlete(ILogger log,
            [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            var authorizationCode = context.GetInput<string>();

            var athleteId = Guid.Empty;
            try
            {
                // 1. Generate athlete id

                athleteId = await context.CallActivityAsync<Guid>(FunctionsNames.A_GenerateAthleteId, null);

                // 2. Exchange and authorize athlete
                var athlete = await context.CallActivityWithRetryAsync<AthleteDto>(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary,
                    new RetryOptions(TimeSpan.FromSeconds(5), 3)
                    {
                        //Handle = ex => !(ex.InnerException is AthleteAlreadyExistsException)
                        Handle = ex => !ex.InnerException.Message.StartsWith(nameof(AthleteAlreadyExistsException)) // temp fix: https://github.com/Azure/azure-functions-durable-extension/issues/84
                    }, new ExchangeTokenAndGetAthleteSummaryInput(athleteId, authorizationCode));
                if (!context.IsReplaying)
                {
                    log.LogInformation(FunctionsNames.O_AuthorizeNewAthlete, $"Exchanged token for user {athlete.FirstName} {athlete.LastName}.");
                }

                // 3. Send approval request
                if (!context.IsReplaying)
                {
                    log.LogInformation(FunctionsNames.O_AuthorizeNewAthlete, "Sending approval email...");
                }
                await context.CallActivityAsync(FunctionsNames.A_SendAthleteApprovalRequest, (athlete.FirstName, athlete.LastName));
                if (!context.IsReplaying)
                {
                    log.LogInformation(FunctionsNames.O_AuthorizeNewAthlete, "Sent approval email.");
                }

                // 4. Wait for approval
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
                        throw new FailedToApproveAthleteTimeoutException(athlete.Id, athlete.ExternalId, timeoutTaskDuration);
                    }
                }

                if (!context.IsReplaying)
                {
                    log.LogInformation(FunctionsNames.O_AuthorizeNewAthlete, $"Athlete: {athlete.FirstName} {athlete.LastName} has been {approvalResult}.");
                }

                // 5. Process a new athlete request
                await context.CallActivityAsync(FunctionsNames.A_ProcessNewAthleteRequest, athlete);
                
                if (!context.IsReplaying)
                {
                    log.LogInformation(FunctionsNames.O_AuthorizeNewAthlete, "Processed athlete's data.");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"[{FunctionsNames.O_AuthorizeNewAthlete}] failed to authorize a new athlete in the system. {ex}";
                log.LogError(errorMessage);
                await context.CallActivityAsync(FunctionsNames.A_AuthorizeNewAthleteCompensation, new AuthorizeNewAthleteCompensation(athleteId, authorizationCode) { ErrorMessage = errorMessage });

                throw;
            }
        }
    }

    public class AuthorizeNewAthleteCompensation
    {
        public Guid AthleteId { get; set; }
        public string AuthorizationCode { get; set; }
        public string ErrorMessage { get; set; }

        public AuthorizeNewAthleteCompensation(Guid athleteId, string authorizationCode)
        {
            AthleteId = athleteId;
            AuthorizationCode = authorizationCode;
        }
    }
}
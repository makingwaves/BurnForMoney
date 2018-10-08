using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
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

            // 1. Generate token and get information about athlete
            var athleteInformation = await context.CallActivityWithRetryAsync<AuthorizeNewAthleteActivities.A_GenerateAccessToken_Output>(FunctionsNames.A_GenerateAccessToken,
                new RetryOptions(TimeSpan.FromSeconds(5), 3), authorizationCode);
            var (athleteFirstName, athleteLastName) =
                (athleteInformation.Athlete.Firstname, athleteInformation.Athlete.Lastname);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_GenerateAccessToken}] generated access token for user {athleteFirstName} " +
                                   $"{athleteLastName}.");
            }

            // 2. Encrypt access token
            var encryptedAccessToken =
                await context.CallActivityAsync<string>(FunctionsNames.A_EncryptAccessToken, athleteInformation.AccessToken);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_EncryptAccessToken}] encrypted access token.");
            }

            // 3. Save athlete in the database
            await context.CallActivityAsync(FunctionsNames.A_AddAthleteToDatabase, (encryptedAccessToken, athleteInformation.Athlete));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_AddAthleteToDatabase}] saved athlete information.");
            }

            // 4. Send approval request
            await context.CallActivityAsync(FunctionsNames.A_SendAthleteApprovalRequest, (athleteFirstName, athleteLastName));

            // 5. Wait for approval
            string approvalResult;
            using (var cts = new CancellationTokenSource())
            {
                var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddDays(2), cts.Token);
                var approvalTask = context.WaitForExternalEvent<string>(EventNames.AthleteApproval);

                var winner = await Task.WhenAny(timeoutTask, approvalTask);
                if (winner == approvalTask)
                {
                    approvalResult = approvalTask.Result;
                    cts.Cancel();
                }
                else
                {
                    approvalResult = AthleteApprovalResult.Timeout.ToString();
                }
            }

            if (approvalResult == AthleteApprovalResult.Approved.ToString())
            {
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.A_SendAthleteApprovalRequest}] Athlete: {athleteFirstName} {athleteLastName} has been approved.");
                }

                // 6. Make a record active.
                await context.CallActivityAsync(FunctionsNames.A_ActivateANewAthlete, athleteInformation.Athlete.Id);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.A_SendAthleteApprovalRequest}] Athlete: {athleteFirstName} {athleteLastName} has been activated.");
                }
            }
            else
            {
                // 6. Anonymize record. It should not be deleted, because foreign key might exist (historical data).
                await context.CallActivityAsync(FunctionsNames.A_AnonymizeRejectedAthlete, athleteInformation.Athlete.Id);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.A_AnonymizeRejectedAthlete}] Athlete: {athleteFirstName} {athleteLastName} has been rejected and anonymized.");
                }
            }
        }
    }

    public enum AthleteApprovalResult
    {
        None,
        Pending,
        Approved,
        Rejected,
        Timeout
    }
}
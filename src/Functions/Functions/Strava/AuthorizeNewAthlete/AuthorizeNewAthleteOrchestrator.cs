using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.Functions.External.Strava.Api.Model;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteOrchestrator
    {
        [FunctionName(FunctionsNames.O_AuthorizeNewAthlete)]
        public static async Task O_AuthorizeNewAthlete(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext,
            [Queue(QueueNames.AuthorizationCodesPoison)] CloudQueue authorizationCodePoisionQueue)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.O_AuthorizeNewAthlete}` received a request.");
            }

            var authorizationCode = context.GetInput<string>();

            // 1. Generate token and get information about athlete
            (string AccessToken, Athlete Athlete) athleteInformation = await context.CallActivityWithRetryAsync<(string, Athlete)>(FunctionsNames.A_GenerateAccessToken,
                new RetryOptions(TimeSpan.FromSeconds(5), 3), authorizationCode);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_GenerateAccessToken}] generated access token for user {athleteInformation.Athlete.Firstname} " +
                                   $"{athleteInformation.Athlete.Lastname}.");
            }

            try
            {
                // 2. Encrypt access token
                var encryptedAccessToken =
                    await context.CallActivityAsync<string>(FunctionsNames.A_EncryptAccessToken, athleteInformation.AccessToken);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.A_EncryptAccessToken}] encrypted access token.");
                }

                // 3. Send approval request
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.A_SendAthleteApprovalRequest}] sending approval email...");
                }
                await context.CallActivityAsync(FunctionsNames.A_SendAthleteApprovalRequest, (athleteInformation.Athlete.Firstname, athleteInformation.Athlete.Lastname));

                // 4. Wait for approval
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
                        log.LogInformation($"[{FunctionsNames.A_SendAthleteApprovalRequest}] Athlete: {athleteInformation.Athlete.Firstname} {athleteInformation.Athlete.Lastname} has been approved.");
                    }

                    // 5. Save athlete
                    var athlete = new Persistence.DatabaseSchema.Athlete
                    {
                        AthleteId = athleteInformation.Athlete.Id,
                        FirstName = athleteInformation.Athlete.Firstname,
                        LastName = athleteInformation.Athlete.Lastname,
                        AccessToken = encryptedAccessToken,
                        Active = true,
                        LastUpdate = context.CurrentUtcDateTime.AddMonths(-3)
                    };
                    await context.CallActivityWithRetryAsync(FunctionsNames.A_AddAthleteToDatabase,
                        new RetryOptions(TimeSpan.FromSeconds(10), 5), athlete);
                    if (!context.IsReplaying)
                    {
                        log.LogInformation($"[{FunctionsNames.A_AddAthleteToDatabase}] saved athlete information.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"[{FunctionsNames.A_AddAthleteToDatabase}] failed to save a new athlete. {ex}");
                await authorizationCodePoisionQueue.AddMessageAsync(new CloudQueueMessage(authorizationCode));
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
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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
            var generateTokenResponse = await context.CallActivityWithRetryAsync<AuthorizeNewAthleteActivities.A_GenerateAccessToken_Output>(FunctionsNames.A_GenerateAccessToken,
                new RetryOptions(TimeSpan.FromSeconds(5), 3), authorizationCode);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] generated access token for user {generateTokenResponse.Athlete.Firstname} {generateTokenResponse.Athlete.Lastname}.");
            }

            // 2. Encrypt access token
            var encryptedAccessToken =
                await context.CallActivityAsync<string>(FunctionsNames.A_EncryptAccessToken, generateTokenResponse.AccessToken);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] encrypted access token.");
            }

            // 3. Save athlete in the database
            await context.CallActivityAsync<bool>(FunctionsNames.A_AddAthleteToDatabase, new AuthorizeNewAthleteActivities.A_AddAthleteToDatabase_Input { Athlete = generateTokenResponse.Athlete, EncryptedAccessToken = encryptedAccessToken});
            if (context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] saved athlete information.");
            }

            // 4. Send approval request
            await context.CallActivityAsync(FunctionsNames.A_SendAthleteApprovalRequest, ActivityInput.Empty);
            var approvalResult = await context.WaitForExternalEvent<AthleteApprovalResult>(EventNames.AthleteApproval);

            if (approvalResult == AthleteApprovalResult.Approved)
            {
                // 5. Make a record active.
                await context.CallActivityAsync(FunctionsNames.A_ActivateANewAthlete, generateTokenResponse.Athlete.Id);
            }
            else
            {
                // 5. Reject
            }
        }
    }

    public enum AthleteApprovalResult
    {
        None,
        Pending,
        Approved,
        Timeout
    }
}
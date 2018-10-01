using System.Threading.Tasks;
using BurnForMoney.Functions.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.AuthorizeNewAthleteFunctions
{
    public static class AuthorizeNewAthleteStarterOrchestrator
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
            var generateTokenResponse = await context.CallActivityAsync<GenerateAccessTokenActivities.A_GenerateAccessToken_Output>(FunctionsNames.A_GenerateAccessToken, authorizationCode);
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

            // 3. Save encrypted access token in database
            await context.CallActivityAsync<string>(FunctionsNames.A_AddAthleteToDatabase, new GenerateAccessTokenActivities.A_AddAthleteToDatabase_Input { Athlete = generateTokenResponse.Athlete, EncryptedAccessToken = encryptedAccessToken});
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_AuthorizeNewAthlete}] saved athlete information.");
            }
        }
    }
}
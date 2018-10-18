using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.Functions.External.Strava.Api.Auth;
using BurnForMoney.Functions.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteOrchestrator
    {
        [FunctionName(FunctionsNames.Strava_O_AuthorizeNewAthlete)]
        public static async Task O_AuthorizeNewAthlete(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext,
            [Queue(QueueNames.AuthorizationCodesPoison)] CloudQueue authorizationCodePoisionQueue,
            [Queue(QueueNames.NewStravaAthletesRequests)] CloudQueue newAthletesRequestsQueue)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation($"Orchestration function `{FunctionsNames.Strava_O_AuthorizeNewAthlete}` received a request.");
            }
            
            var authorizationCode = context.GetInput<string>();
            
            try
            {
                // 1. Exchange authorization code
                var tokenExchangeResult = await context.CallActivityWithRetryAsync<TokenExchangeResult>(FunctionsNames.Strava_A_ExchangeToken,
                    new RetryOptions(TimeSpan.FromSeconds(5), 3), authorizationCode);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] exchanged token for user {tokenExchangeResult.Athlete.Firstname} " +
                                       $"{tokenExchangeResult.Athlete.Lastname}.");
                }

                // 2. Send approval request
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] sending approval email...");
                }
                await context.CallActivityAsync(FunctionsNames.Strava_A_SendAthleteApprovalRequest, (tokenExchangeResult.Athlete.Firstname, tokenExchangeResult.Athlete.Lastname));
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] sent approval email.");
                }

                // 3. Wait for approval
                string approvalResult;
                using (var cts = new CancellationTokenSource())
                {
                    var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddDays(3), cts.Token);
                    var approvalTask = context.WaitForExternalEvent<string>("AthleteApproval");

                    var winner = await Task.WhenAny(timeoutTask, approvalTask);
                    if (winner == approvalTask)
                    {
                        approvalResult = approvalTask.Result;
                        cts.Cancel();
                    }
                    else
                    {
                        throw new TimeoutException("Failed to approve athlete in the allotted time period.");
                    }
                }

                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] Athlete: {tokenExchangeResult.Athlete.Firstname} {tokenExchangeResult.Athlete.Lastname} has been {approvalResult}.");
                }

                // 4. Encrypt tokens
                var encryptedAccessToken =
                    await context.CallActivityAsync<string>(FunctionsNames.Strava_A_EncryptToken, tokenExchangeResult.AccessToken);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] encrypted access token.");
                }
                var encryptedRefreshToken =
                    await context.CallActivityAsync<string>(FunctionsNames.Strava_A_EncryptToken, tokenExchangeResult.RefreshToken);
                if (!context.IsReplaying)
                {
                    log.LogInformation($"[{FunctionsNames.Strava_A_EncryptToken}] encrypted refresh token.");
                }

                // 5. Process a new athlete request
                var athlete = new NewStravaAthlete
                {
                    AthleteId = tokenExchangeResult.Athlete.Id,
                    FirstName = tokenExchangeResult.Athlete.Firstname,
                    LastName = tokenExchangeResult.Athlete.Lastname,
                    ProfilePictureUrl = tokenExchangeResult.Athlete.Profile,
                    EncryptedAccessToken = encryptedAccessToken,
                    EncryptedRefreshToken = encryptedRefreshToken,
                    TokenExpirationDate = tokenExchangeResult.ExpiresAt
                };

                var json = JsonConvert.SerializeObject(athlete);
                await newAthletesRequestsQueue.AddMessageAsync(new CloudQueueMessage(json));
            }
            catch (Exception ex)
            {
                log.LogError($"[{FunctionsNames.Strava_O_AuthorizeNewAthlete}] failed to authorize a new athlete in the system. {ex}");
                await authorizationCodePoisionQueue.AddMessageAsync(new CloudQueueMessage(authorizationCode));
            }
        }
    }
}
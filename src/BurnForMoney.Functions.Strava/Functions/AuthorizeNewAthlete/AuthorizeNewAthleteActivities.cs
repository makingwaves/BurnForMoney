using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary)]
        public static StravaAthlete A_ExchangeTokenAndGetAthleteSummary([ActivityTrigger]string authorizationCode, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");

            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authorizationCode);

            log.LogFunctionEnd(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary);
            return new StravaAthlete
            {
                AthleteId = response.Athlete.Id,
                FirstName = response.Athlete.Firstname,
                LastName = response.Athlete.Lastname,
                ProfilePictureUrl = response.Athlete.Profile,
                EncryptedAccessToken = AccessTokensEncryptionService.Encrypt(response.AccessToken,
                    configuration.Strava.AccessTokensEncryptionKey),
                EncryptedRefreshToken = AccessTokensEncryptionService.Encrypt(response.RefreshToken,
                    configuration.Strava.AccessTokensEncryptionKey),
                TokenExpirationDate = response.ExpiresAt
            };
        }

        [FunctionName(FunctionsNames.A_SendAthleteApprovalRequest)]
        public static async Task A_SendAthleteApprovalRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            [Queue(AppQueueNames.NotificationsToSend, Connection = "AppQueuesStorage")] CloudQueue notificationsQueue,
            [Table("AthleteApprovals", "AzureWebJobsStorage")] IAsyncCollector<AthleteApproval> athleteApprovalCollector,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.A_SendAthleteApprovalRequest);
            var (firstName, lastName) = activityContext.GetInput<(string, string)>();
            
            var approvalCode = Guid.NewGuid().ToString("N");
            var athleteApproval = new AthleteApproval
            {
                PartitionKey = "AthleteApproval",
                RowKey = approvalCode,
                OrchestrationId = activityContext.InstanceId
            };

            var approvalFunctionAddress = $"{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
            var notification = new Notification
            {
                Recipients = new List<string> { configuration.Email.AthletesApprovalEmail },
                Subject = "Athlete is awaiting approval",
                HtmlContent = $@"
<p>Hi there,</p>
<p>Please review a new authorization request. Athlete: {firstName} {lastName}.</p>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Approved.ToString()}\">Approve</a><br>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Rejected.ToString()}\">Reject</a>"
            };

            log.LogInformation(FunctionsNames.A_SendAthleteApprovalRequest, $"Sending approval request for athlete {firstName} {lastName} to: {configuration.Email.AthletesApprovalEmail}.");
            await athleteApprovalCollector.AddAsync(athleteApproval);
            await notificationsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(notification)));

            log.LogFunctionEnd(FunctionsNames.A_SendAthleteApprovalRequest);
        }

        [FunctionName(FunctionsNames.A_ProcessNewAthleteRequest)]
        public static async Task A_ProcessNewAthleteRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(QueueNames.NewStravaAthletesRequests)] CloudQueue newAthletesRequestsQueue)
        {
            log.LogFunctionStart(FunctionsNames.A_ProcessNewAthleteRequest);

            var athlete = activityContext.GetInput<StravaAthlete>();
            var json = JsonConvert.SerializeObject(athlete);
            await newAthletesRequestsQueue.AddMessageAsync(new CloudQueueMessage(json));

            log.LogFunctionEnd(FunctionsNames.A_ProcessNewAthleteRequest);
        }

        [FunctionName(FunctionsNames.A_AuthorizeNewAthleteCompensation)]
        public static async Task A_AuthorizeNewAthleteCompensation([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(QueueNames.AuthorizationCodesPoison)] CloudQueue authorizationCodePoisonQueue)
        {
            log.LogFunctionStart(FunctionsNames.A_AuthorizeNewAthleteCompensation);

            var input = activityContext.GetInput<AuthorizeNewAthleteCompensation>();
            var json = JsonConvert.SerializeObject(input);
            await authorizationCodePoisonQueue.AddMessageAsync(new CloudQueueMessage(json));

            log.LogFunctionEnd(FunctionsNames.A_AuthorizeNewAthleteCompensation);
        }
    }

    internal enum AthleteApprovalResult
    {
        Approved,
        Rejected
    }

    public class AthleteApproval
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
    }
}

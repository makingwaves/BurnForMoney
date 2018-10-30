using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Strava_A_ExchangeTokenAndGetAthleteSummary)]
        public static NewStravaAthlete Strava_A_ExchangeTokenAndGetAthleteSummary([ActivityTrigger]string authorizationCode, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_ExchangeTokenAndGetAthleteSummary} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(context);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");

            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authorizationCode);

            return new NewStravaAthlete
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
        
        [FunctionName(FunctionsNames.Strava_A_SendAthleteApprovalRequest)]
        public static async Task A_SendAthleteApprovalRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [SendGrid(ApiKey = "SendGrid:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            [Table("AthleteApprovals", "AzureWebJobsStorage")] IAsyncCollector<AthleteApproval> athleteApprovalCollector)
        {
            var (firstName, lastName) = activityContext.GetInput<(string, string)>();

            var configuration = ApplicationConfiguration.GetSettings(context);

            var approvalCode = Guid.NewGuid().ToString("N");
            var athleteApproval = new AthleteApproval
            {
                PartitionKey = "AthleteApproval",
                RowKey = approvalCode,
                OrchestrationId = activityContext.InstanceId
            };

            var message = new SendGridMessage
            {
                From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money")
            };
            message.AddTo(new EmailAddress(configuration.Email.AthletesApprovalEmail));
            message.Subject = "Athlete is awaiting approval";

            var approvalFunctionAddress = $"{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
            message.HtmlContent = $"Please review a new authorization request. Athlete: {firstName} {lastName}.<br>" +
                $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Approved.ToString()}\">Approve</a><br>" +
                $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Rejected.ToString()}\">Reject</a>";

            log.LogInformation($"Sending approval request for athlete {firstName} {lastName} to: {configuration.Email.AthletesApprovalEmail}.");
            await athleteApprovalCollector.AddAsync(athleteApproval);
            await messageCollector.AddAsync(message);
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

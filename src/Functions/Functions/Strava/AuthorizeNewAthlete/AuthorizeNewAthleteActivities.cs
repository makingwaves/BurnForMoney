using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.External.Strava.Api.Auth;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Strava_A_ExchangeToken)]
        public static async Task<TokenExchangeResult> A_ExchangeToken([ActivityTrigger]string authorizationCode, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_ExchangeToken} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");

            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authorizationCode);
            return response;
        }

        [FunctionName(FunctionsNames.Strava_A_EncryptToken)]
        public static async Task<string> A_EncryptTokenAsync([ActivityTrigger]string token, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.Strava_A_EncryptToken} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = secret.Value;

            var encryptedToken = Cryptography.EncryptString(token, accessTokenEncryptionKey);
            log.LogInformation("Access token has been encrypted.");
            return encryptedToken;
        }

        [FunctionName(FunctionsNames.Strava_A_SendAthleteApprovalRequest)]
        public static async Task A_SendAthleteApprovalRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [SendGrid(ApiKey = "SendGrid.ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            [Table("AthleteApprovals", "AzureWebJobsStorage")] IAsyncCollector<AthleteApproval> athleteApprovalCollector)
        {
            var (firstName, lastName) = activityContext.GetInput<(string, string)>();

            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

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

            var protocol = configuration.IsLocalEnvironment ? "http" : "https";
            var approvalFunctionAddress = $"{protocol}://{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
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

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Athlete = BurnForMoney.Functions.External.Strava.Api.Model.Athlete;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.A_GenerateAccessToken)]
        public static async Task<(string, Athlete)> A_GenerateAccessToken([ActivityTrigger]string authorizationCode, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_GenerateAccessToken} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");

            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authorizationCode);
            return (response.AccessToken, response.Athlete);
        }

        [FunctionName(FunctionsNames.A_EncryptAccessToken)]
        public static async Task<string> A_EncryptAccessTokenAsync([ActivityTrigger]string accessToken, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_EncryptAccessToken} function processed a request.");
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(configuration.ConnectionStrings.KeyVaultConnectionString, KeyVaultSecretNames.StravaTokensEncryptionKey)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = secret.Value;

            var encryptedToken = Cryptography.EncryptString(accessToken, accessTokenEncryptionKey);
            log.LogInformation("Access token has been encrypted.");
            return encryptedToken;
        }

        [FunctionName(FunctionsNames.A_AddAthleteToDatabase)]
        public static async Task A_AddAthleteToDatabaseAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context)
        {
            var athlete = activityContext.GetInput<Persistence.DatabaseSchema.Athlete>();
            var configuration = await ApplicationConfiguration.GetSettingsAsync(context);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("Strava_Athlete_Upsert", athlete, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation($"Athlete: {athlete.FirstName} {athlete.LastName} has been saved successfully");
                }
                else
                {
                    throw new Exception("An error occurred while saving athlete data.");
                }
            }
        }

        [FunctionName(FunctionsNames.A_SendAthleteApprovalRequest)]
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

            var message = new SendGridMessage();
            message.From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money");
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

    public class AthleteApproval
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
    }
}

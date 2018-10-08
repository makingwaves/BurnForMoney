using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api;
using BurnForMoney.Functions.Helpers;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Athlete = BurnForMoney.Functions.External.Strava.Api.Model.Athlete;

namespace BurnForMoney.Functions.Functions.Strava.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        [FunctionName(FunctionsNames.A_GenerateAccessToken)]
        public static A_GenerateAccessToken_Output A_GenerateAccessToken([ActivityTrigger]string authorizationCode, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{FunctionsNames.A_GenerateAccessToken} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(context);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");

            var stravaService = new StravaService();
            var response = stravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, authorizationCode);
            return new A_GenerateAccessToken_Output
            {
                Athlete = response.Athlete,
                AccessToken = response.AccessToken
            };
        }

        public class A_GenerateAccessToken_Output
        {
            public Athlete Athlete { get; set; }
            public string AccessToken { get; set; }
        }

        [FunctionName(FunctionsNames.A_AddAthleteToDatabase)]
        public static async Task A_AddAthleteToDatabaseAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context)
        {
            var (encryptedAccessToken, athlete) = activityContext.GetInput<(string, Athlete)>();
            var configuration = ApplicationConfiguration.GetSettings(context);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("Strava_Athlete_Upsert",
                        new Persistence.DatabaseSchema.Athlete
                        {
                            AthleteId = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            AccessToken = encryptedAccessToken,
                            Active = false
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation($"Athlete: {athlete.Firstname} {athlete.Lastname} has been saved successfully");
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
            var (athleteFirstName, athleteLastName) = activityContext.GetInput<(string, string)>();

            var configuration = ApplicationConfiguration.GetSettings(context);

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
            
            var protocol = "https";
            var approvalFunctionAddress = $"{protocol}://{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
            message.HtmlContent = $"Please review a new authorization request. Athlete: {athleteFirstName} {athleteLastName}.<br>" +
                $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Approved.ToString()}\">Approve</a><br>" +
                $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Rejected.ToString()}\">Reject</a>";

            log.LogInformation($"Sending approval request for athlete {athleteFirstName} {athleteLastName} to: {configuration.Email.AthletesApprovalEmail}.");
            await athleteApprovalCollector.AddAsync(athleteApproval);
            await messageCollector.AddAsync(message);
        }

        [FunctionName(FunctionsNames.A_ActivateANewAthlete)]
        public static async Task A_ActivateANewAthleteAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context)
        {
            var athleteId = activityContext.GetInput<int>();

            var configuration = ApplicationConfiguration.GetSettings(context);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET Active='1' WHERE AthleteId=@AthleteId",
                    new { AthleteId = athleteId });
                if (affectedRows != 1)
                {
                    throw new Exception($"Failed to activate athlete with id: {athleteId}");
                }
            }
        }

        [FunctionName(FunctionsNames.A_AnonymizeRejectedAthlete)]
        public static async Task A_AnonymizeRejectedAthlete([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context)
        {
            var athleteId = activityContext.GetInput<int>();

            var configuration = ApplicationConfiguration.GetSettings(context);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET FirstName=@FirstName, LastName=@LastName, AccessToken=@AccessToken, Active=@Active WHERE AthleteId=@AthleteId",
                    new
                    {
                        AthleteId = athleteId,
                        FirstName = "Rejected",
                        LastName = "Rejected",
                        AccessToken = "Rejected",
                        Active = false
                    });
                if (affectedRows != 1)
                {
                    throw new Exception($"Failed to delete rejected athlete with id: {athleteId}");
                }
            }
        }
    }

    public class AthleteApproval
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
    }
}

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
            var request = activityContext.GetInput<A_AddAthleteToDatabase_Input>();
            var athlete = request.Athlete;
            var encryptedAccessToken = request.EncryptedAccessToken;
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
            ExecutionContext context, [SendGrid(ApiKey = "SendGrid.ApiKey")] IAsyncCollector<SendGridMessage> messageCollector)
        {
            var configuration = ApplicationConfiguration.GetSettings(context);

            var message = new SendGridMessage();
            message.From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money");
            message.AddTo(new EmailAddress(configuration.Email.MainRecipientEmail, configuration.Email.MainRecipientEmail));
            message.Subject = "[subject]";
            message.HtmlContent = "[content]";
            message.PlainTextContent = "<strong>[content]</strong>";
                
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

        public class A_AddAthleteToDatabase_Input
        {
            public Athlete Athlete { get; set; }
            public string EncryptedAccessToken { get; set; }
        }
    }
}

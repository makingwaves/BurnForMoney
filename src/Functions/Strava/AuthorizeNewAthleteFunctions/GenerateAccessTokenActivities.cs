using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.DatabaseSchema;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.AuthorizeNewAthleteFunctions
{
    public static class GenerateAccessTokenActivities
    {
        [FunctionName(FunctionsNames.A_GenerateAccessToken)]
        public static async Task<A_GenerateAccessToken_Output> A_GenerateAccessToken([ActivityTrigger]string authorizationCode, ILogger log,
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
            public Api.Model.Athlete Athlete { get; set; }
            public string AccessToken { get; set; }
        }

        [FunctionName(FunctionsNames.A_AddAthleteToDatabase)]
        public static async Task A_AddAthleteToDatabase([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context)
        {
            var request = activityContext.GetInput<A_AddAthleteToDatabase_Input>();
            var athlete = request.Athlete;
            var encryptedAccessToken = request.EncryptedAccessToken;
            var configuration = ApplicationConfiguration.GetSettings(context);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.ExecuteAsync("Strava_Athlete_Upsert",
                        new Athlete
                        {
                            AthleteId = athlete.Id,
                            FirstName = athlete.Firstname,
                            LastName = athlete.Lastname,
                            AccessToken = encryptedAccessToken,
                            Active = true
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                log.LogInformation($"Athlete: {athlete.Firstname} {athlete.Lastname} has been saved successfully");
            }
        }

        public class A_AddAthleteToDatabase_Input
        {
            public Api.Model.Athlete Athlete { get; set; }
            public string EncryptedAccessToken { get; set; }
        }


    }
}

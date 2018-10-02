using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Support
{
    public static class AthleteOperations
    {
        [FunctionName(FunctionsNames.Support_Strava_Athlete_Deactivate)]
        public static async Task<IActionResult> Support_Strava_DeactivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/strava/athlete/deactivate")]HttpRequest req, ILogger log,
            ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Athlete_Deactivate} function processed a request.");

            string athleteId = req.Query["athleteId"];
            if (string.IsNullOrWhiteSpace(athleteId))
            {
                log.LogWarning("Function invoked with incorrect parameters. [athleteId] is null or empty.");
                return new BadRequestObjectResult("AthleteId is required.");
            }

            var connectionString = ApplicationConfiguration.GetSettings(executionContext).ConnectionStrings
                .SqlDbConnectionString;

            var deactivationResult = await DeactivateAthleteAsync(athleteId, connectionString);
            if (deactivationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been deactivated.");
            }

            return new BadRequestResult();
        }

        [FunctionName(FunctionsNames.Support_Strava_Athlete_Activate)]
        public static async Task<IActionResult> Support_Strava_ActivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/strava/athlete/activate")]HttpRequest req, ILogger log,
            ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.Support_Strava_Athlete_Activate} function processed a request.");

            string athleteId = req.Query["athleteId"];
            if (string.IsNullOrWhiteSpace(athleteId))
            {
                log.LogWarning("Function invoked with incorrect parameters. [athleteId] is null or empty.");
                return new BadRequestObjectResult("AthleteId is required.");
            }

            var connectionString = ApplicationConfiguration.GetSettings(executionContext).ConnectionStrings
                .SqlDbConnectionString;

            var activationResult = await ActivateAthleteAsync(athleteId, connectionString);
            if (activationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been activated.");
            }

            return new BadRequestResult();
        }

        private static async Task<bool> DeactivateAthleteAsync(string athleteId, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET Active='0' WHERE AthleteId=@AthleteId",
                    new { AthleteId = athleteId });
                return affectedRows == 1;
            }
        }

        private static async Task<bool> ActivateAthleteAsync(string athleteId, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET Active='1' WHERE AthleteId=@AthleteId",
                    new { AthleteId = athleteId });
                return affectedRows == 1;
            }
        }
    }
}
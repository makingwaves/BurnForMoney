using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class AthleteOperations
    {
        [FunctionName(FunctionsNames.Support_Athlete_Deactivate)]
        public static async Task<IActionResult> Support_DeactivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:int}/deactivate")]HttpRequest req, ILogger log,
            ExecutionContext executionContext, string athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Athlete_Deactivate} function processed a request.");

            if (string.IsNullOrWhiteSpace(athleteId))
            {
                log.LogWarning("Function invoked with incorrect parameters. [athleteId] is null or empty.");
                return new BadRequestObjectResult("AthleteId is required.");
            }

            var connectionString = (ApplicationConfiguration.GetSettings(executionContext)).ConnectionStrings
                .SqlDbConnectionString;

            var deactivationResult = await DeactivateAthleteAsync(athleteId, connectionString);
            if (deactivationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been deactivated.");
            }

            return new BadRequestResult();
        }

        [FunctionName(FunctionsNames.Support_Athlete_Activate)]
        public static async Task<IActionResult> Support_ActivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:int}/activate")]HttpRequest req, ILogger log,
            ExecutionContext executionContext, string athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Athlete_Activate} function processed a request.");

            if (string.IsNullOrWhiteSpace(athleteId))
            {
                log.LogWarning("Function invoked with incorrect parameters. [athleteId] is null or empty.");
                return new BadRequestObjectResult("AthleteId is required.");
            }

            var connectionString = (ApplicationConfiguration.GetSettings(executionContext)).ConnectionStrings
                .SqlDbConnectionString;

            var activationResult = await ActivateAthleteAsync(athleteId, connectionString);
            if (activationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been activated.");
            }

            return new BadRequestResult();
        }

        [FunctionName(FunctionsNames.Support_Athlete_Delete)]
        public static async Task<IActionResult> Support_Athlete_Delete([HttpTrigger(AuthorizationLevel.Admin, "delete", Route = "support/athlete/{athleteId:int}/delete")]HttpRequest req, ILogger log,
            ExecutionContext executionContext, string athleteId)
        {
            log.LogInformation($"{FunctionsNames.Support_Athlete_Delete} function processed a request.");

            if (string.IsNullOrWhiteSpace(athleteId))
            {
                log.LogWarning("Function invoked with incorrect parameters. [athleteId] is null or empty.");
                return new BadRequestObjectResult("AthleteId is required.");
            }

            var connectionString = (ApplicationConfiguration.GetSettings(executionContext)).ConnectionStrings
                .SqlDbConnectionString;

            try
            {
                await DeleteAthleteAsync(athleteId, connectionString, log);
                return new OkObjectResult($"Athlete with id: {athleteId} has been deleted.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
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

        private static async Task DeleteAthleteAsync(string athleteId, string connectionString, ILogger log)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                log.LogInformation("Beginning a new database transaction...");
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await conn.ExecuteAsync(
                            "DELETE FROM dbo.[Strava.AccessTokens] WHERE AthleteId=@AthleteId",
                            new { AthleteId = athleteId }, transaction);
                        log.LogInformation("Removed access token.");

                        var removedActivities = await conn.ExecuteAsync(
                            "DELETE FROM dbo.[Activities] WHERE AthleteId=@AthleteId",
                            new { AthleteId = athleteId }, transaction);
                        log.LogInformation($"Removed {removedActivities} activities.");

                        var affectedRows = await conn.ExecuteAsync(
                            "DELETE FROM dbo.[Athletes] WHERE Id=@AthleteId",
                            new { AthleteId = athleteId }, transaction);
                        log.LogInformation("Removed athlete information.");

                        if (affectedRows != 1)
                        {
                            throw new Exception($"Failed to delete athlete: [{athleteId}]");
                        }

                        transaction.Commit();
                        log.LogInformation("Commited database transaction.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        log.LogError($"Error occuring during processing a new athlete. {ex.Message}", ex);
                        throw;
                    }
                }
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
using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions._Support
{
    public static class AthleteOperationsFunc
    {
        [FunctionName(SupportFunctionsNames.DeactivateAthlete)]
        public static async Task<IActionResult> DeactivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:length(32)}/deactivate")]HttpRequest req, ILogger log,
            string athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.DeactivateAthlete);

            var connectionString = (ApplicationConfiguration.GetSettings()).ConnectionStrings
                .SqlDbConnectionString;

            var deactivationResult = await DeactivateAthleteAsync(athleteId, connectionString);
            if (deactivationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been deactivated.");
            }

            log.LogFunctionEnd(SupportFunctionsNames.DeactivateAthlete);
            return new BadRequestResult();
        }

        [FunctionName(SupportFunctionsNames.ActivateAthlete)]
        public static async Task<IActionResult> ActivateAthlete([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "support/athlete/{athleteId:length(32)}/activate")]HttpRequest req, ILogger log,
            string athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.ActivateAthlete);

            var connectionString = (ApplicationConfiguration.GetSettings()).ConnectionStrings
                .SqlDbConnectionString;

            var activationResult = await ActivateAthleteAsync(athleteId, connectionString);
            if (activationResult)
            {
                return new OkObjectResult($"Athlete with id: {athleteId} has been activated.");
            }

            log.LogFunctionEnd(SupportFunctionsNames.ActivateAthlete);
            return new BadRequestResult();
        }

        [FunctionName(SupportFunctionsNames.DeleteAthlete)]
        public static async Task<IActionResult> DeleteAthlete([HttpTrigger(AuthorizationLevel.Admin, "delete", Route = "support/athlete/{athleteId:int:min(1)}")]HttpRequest req, ILogger log,
            int athleteId)
        {
            log.LogFunctionStart(SupportFunctionsNames.DeleteAthlete);

            var connectionString = (ApplicationConfiguration.GetSettings()).ConnectionStrings
                .SqlDbConnectionString;

            try
            {
                await DeleteAthleteAsync(athleteId, connectionString, log);
                log.LogFunctionEnd(SupportFunctionsNames.DeleteAthlete);
                return new OkObjectResult($"Athlete with id: {athleteId} has been deleted.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private static async Task<bool> DeactivateAthleteAsync(string athleteId, string connectionString)
        {
            using (var conn = SqlConnectionFactory.Create(connectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET Active='0' WHERE Id=@AthleteId",
                    new { AthleteId = athleteId });
                return affectedRows == 1;
            }
        }

        private static async Task DeleteAthleteAsync(int athleteId, string connectionString, ILogger log)
        {
            using (var conn = SqlConnectionFactory.Create(connectionString))
            {
                await conn.OpenWithRetryAsync();

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
                            throw new FailedToDeleteAthleteException(athleteId.ToString());
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
            using (var conn = SqlConnectionFactory.Create(connectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(
                    "UPDATE dbo.[Strava.Athletes] SET Active='1' WHERE Id=@AthleteId",
                    new { AthleteId = athleteId });
                return affectedRows == 1;
            }
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class ProcessNewAthlete
    {
        [FunctionName(FunctionsNames.Strava_Q_ProcessNewAthlete)]
        public static async Task Q_ProcessNewAthleteAsync(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(QueueNames.NewStravaAthletesRequests)] NewStravaAthlete athlete)
        {
            log.LogInformation($"{FunctionsNames.Strava_Q_ProcessNewAthlete} function processed a request.");

            var configuration = await ApplicationConfiguration.GetSettingsAsync(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                conn.Open();
                log.LogInformation("Beginning a new database transaction...");
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var sql = @"INSERT INTO dbo.Athletes(ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
                                    OUTPUT INSERTED.[Id]
                                    VALUES(@AthleteId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)";
                        var id = await conn.QuerySingleAsync<int>(sql, new
                        {
                            athlete.AthleteId,
                            athlete.FirstName,
                            athlete.LastName,
                            athlete.ProfilePictureUrl,
                            Active = true,
                            System = "Strava"
                        }, transaction);
                        if (id < 1)
                        {
                            throw new Exception("Failed to add a new athlete.");
                        }
                        log.LogInformation($"Inserted athlete with id: {id}.");

                        sql = @"INSERT INTO dbo.[Strava.AccessTokens](AthleteId, AuthorizationCode, AccessToken, RefreshToken, ExpiresAt)
                                    OUTPUT INSERTED.[Id]
                                    VALUES(@AthleteId, @AuthorizationCode, @AccessToken, @RefreshToken, @ExpiresAt)";
                        var affectedRows = await conn.ExecuteAsync(sql, new
                        {
                            AthleteId = id,
                            AuthorizationCode = athlete.EncryptedAuthorizationCode,
                            AccessToken = athlete.EncryptedAccessToken,
                            RefreshToken = athlete.EncryptedRefreshToken,
                            ExpiresAt = athlete.TokenExpirationDate
                        }, transaction);
                        if (affectedRows != 1)
                        {
                            throw new Exception("Failed to insert access tokens.");
                        }
                        log.LogInformation($"Inserted access tokens for athlete with id: {id}.");

                        transaction.Commit();
                        log.LogInformation("Commited database transaction.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        log.LogError($"Error occuring during processing a new athlete. {ex.Message}", ex);
                        throw ex;
                    }
                }
            }
        }
    }
}
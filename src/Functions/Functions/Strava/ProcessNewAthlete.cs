using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava
{
    public static class ProcessNewAthlete
    {
        [FunctionName(FunctionsNames.Strava_Q_ProcessNewAthlete)]
        public static async Task Q_ProcessNewAthleteAsync(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(QueueNames.NewStravaAthletesRequests)] NewStravaAthlete athlete,
            [Queue(QueueNames.NewStravaAthletesRequestsPoison)] CloudQueue newAthletesRequestPoisonQueue,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues)
        {
            log.LogInformation($"{FunctionsNames.Strava_Q_ProcessNewAthlete} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                conn.Open();

                var id = await conn.QuerySingleOrDefaultAsync<int>("SELECT Id FROM dbo.Athletes WHERE ExternalId=@AthleteId", new { athlete.AthleteId });
                if (id > 0)
                {
                    log.LogError($"Athlete with id: {athlete.AthleteId} already exists.");
                    await newAthletesRequestPoisonQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(athlete)));
                    return;
                }

                int athleteId;
                log.LogInformation("Beginning a new database transaction...");
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var sql = @"INSERT INTO dbo.Athletes(ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
                                    OUTPUT INSERTED.[Id]
                                    VALUES(@AthleteId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)";
                        athleteId = await conn.QuerySingleAsync<int>(sql, new
                        {
                            athlete.AthleteId,
                            athlete.FirstName,
                            athlete.LastName,
                            athlete.ProfilePictureUrl,
                            Active = true,
                            System = "Strava"
                        }, transaction);
                        if (athleteId < 1)
                        {
                            throw new Exception("Failed to add a new athlete.");
                        }
                        log.LogInformation($"Inserted athlete with id: {athleteId}.");

                        sql = @"INSERT INTO dbo.[Strava.AccessTokens](AthleteId, AccessToken, RefreshToken, ExpiresAt)
                                    VALUES(@AthleteId, @AccessToken, @RefreshToken, @ExpiresAt)";
                        var affectedRows = await conn.ExecuteAsync(sql, new
                        {
                            AthleteId = athleteId,
                            AccessToken = athlete.EncryptedAccessToken,
                            RefreshToken = athlete.EncryptedRefreshToken,
                            ExpiresAt = athlete.TokenExpirationDate
                        }, transaction);
                        if (affectedRows != 1)
                        {
                            throw new Exception("Failed to insert access tokens.");
                        }
                        log.LogInformation($"Inserted access tokens for athlete with id: {athleteId}.");

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

                await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(athleteId.ToString()));
            }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete.Dto;
using BurnForMoney.Functions.Strava.Functions.Dto;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.ProcessNewAthlete
{
    public static class ProcessNewAthleteFunc
    {
        [FunctionName(FunctionsNames.Q_ProcessNewAthlete)]
        public static async Task Q_ProcessNewAthleteAsync(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(QueueNames.NewStravaAthletesRequests)] StravaAthlete athlete,
            [Queue(QueueNames.NewStravaAthletesRequestsPoison)] CloudQueue newAthletesRequestPoisonQueue,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues)
        {
            log.LogFunctionStart(FunctionsNames.Q_ProcessNewAthlete);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                conn.Open();
                
                int athleteId;
                log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, "Beginning a new database transaction...");
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        athleteId = await UpsertAthlete(athlete, conn, transaction);
                        if (athleteId < 1)
                        {
                            throw new FailedToAddNewAthleteException(athlete.AthleteId.ToString());
                        }
                        log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, $"Inserted athlete with id: {athleteId}.");

                        var result = await UpsertAccessToken(athleteId, athlete, conn, transaction);
                        if (!result)
                        {
                            throw new FailedToAddAccessTokenException(athleteId.ToString());
                        }
                        log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, $"Inserted access tokens for athlete with id: {athleteId}.");

                        transaction.Commit();
                        log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, "Commited database transaction.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        log.LogError($"[{FunctionsNames.Q_ProcessNewAthlete}] Error occuring during processing a new athlete. {ex.Message}", ex);
                        throw;
                    }
                }

                await Task.Delay(1000);

                var input = new CollectAthleteActivitiesInput
                {
                    AthleteId = athleteId
                };
                var json = JsonConvert.SerializeObject(input);
                await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));
                log.LogFunctionEnd(FunctionsNames.Q_ProcessNewAthlete);
            }
        }

        private static async Task<int> UpsertAthlete(StravaAthlete athlete, IDbConnection conn, IDbTransaction transaction)
        {
            const string sql = @"
    IF EXISTS (SELECT * FROM dbo.Athletes WITH (UPDLOCK) WHERE ExternalId=@AthleteId)
      UPDATE dbo.Athletes
         SET FirstName = @FirstName, LastName = @LastName, ProfilePictureUrl = @ProfilePictureUrl, Active = @Active, System = @System
         OUTPUT INSERTED.[Id]
       WHERE ExternalId = @AthleteId;
    ELSE 
      INSERT INTO dbo.Athletes(ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
                                    OUTPUT INSERTED.[Id]
                                    VALUES(@AthleteId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)";
            return await conn.QuerySingleAsync<int>(sql, new
            {
                athlete.AthleteId,
                athlete.FirstName,
                athlete.LastName,
                athlete.ProfilePictureUrl,
                Active = true,
                System = "Strava"
            }, transaction);
        }

        private static async Task<bool> UpsertAccessToken(int athleteId, StravaAthlete athlete, IDbConnection conn, IDbTransaction transaction)
        {
            const string sql = @"
    IF EXISTS (SELECT * FROM dbo.[Strava.AccessTokens] WITH (UPDLOCK) WHERE AthleteId=@AthleteId)
      UPDATE dbo.[Strava.AccessTokens]
         SET AccessToken = @AccessToken, RefreshToken = @RefreshToken, ExpiresAt = @ExpiresAt
       WHERE AthleteId = @AthleteId;
    ELSE 
      INSERT INTO dbo.[Strava.AccessTokens](AthleteId, AccessToken, RefreshToken, ExpiresAt)
                                    VALUES(@AthleteId, @AccessToken, @RefreshToken, @ExpiresAt)";
            var affectedRows = await conn.ExecuteAsync(sql, new
            {
                AthleteId = athleteId,
                AccessToken = athlete.EncryptedAccessToken,
                RefreshToken = athlete.EncryptedRefreshToken,
                ExpiresAt = athlete.TokenExpirationDate
            }, transaction);
            return affectedRows == 1;
        }
    }
}
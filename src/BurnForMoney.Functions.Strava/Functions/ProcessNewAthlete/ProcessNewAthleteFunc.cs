using System;
using System.Data;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.Functions.CollectAthleteActivitiesFromStravaFunc.Dto;
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
        public static async Task Q_ProcessNewAthleteAsync(ILogger log,
            [QueueTrigger(QueueNames.NewStravaAthletesRequests)] Athlete athlete,
            [Queue(QueueNames.NewStravaAthletesRequestsPoison)] CloudQueue newAthletesRequestPoisonQueue,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_ProcessNewAthlete);

            string athleteId;
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();
                
                log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, "Beginning a new database transaction...");
                try
                {
                    athleteId = await UpsertAthlete(athlete, conn);
                    if (string.IsNullOrWhiteSpace(athleteId))
                    {
                        throw new FailedToAddAthleteException(athlete.Id);
                    }
                    log.LogInformation(FunctionsNames.Q_ProcessNewAthlete, $"Inserted athlete with id: {athleteId}.");
                }
                catch (Exception ex)
                {
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

        private static async Task<string> UpsertAthlete(Athlete athlete, IDbConnection conn)
        {
            const string sql = @"
    IF EXISTS (SELECT * FROM dbo.Athletes WITH (UPDLOCK) WHERE ExternalId=@ExternalId)
      UPDATE dbo.Athletes
         SET FirstName = @FirstName, LastName = @LastName, ProfilePictureUrl = @ProfilePictureUrl, Active = @Active, System = @System
         OUTPUT INSERTED.[Id]
       WHERE ExternalId = @ExternalId;
    ELSE 
      INSERT INTO dbo.Athletes(Id, ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
                                    OUTPUT INSERTED.[Id]
                                    VALUES(@Id, @ExternalId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)";

            return await conn.QuerySingleAsync<string>(sql, new
            {
                athlete.Id,
                athlete.ExternalId,
                athlete.FirstName,
                athlete.LastName,
                athlete.ProfilePictureUrl,
                Active = true,
                System = "Strava"
            });
        }
    }
}
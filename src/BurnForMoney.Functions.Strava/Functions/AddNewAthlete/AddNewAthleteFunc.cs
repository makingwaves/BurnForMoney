using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Strava.Functions.CollectAthleteActivitiesFromStravaFunc.Dto;
using BurnForMoney.Functions.Strava.Functions.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.AddNewAthlete
{
    public static class AddNewAthleteFunc
    {
        [FunctionName(FunctionsNames.Q_ProcessNewAthlete)]
        public static async Task Q_ProcessNewAthlete(ILogger log,
            [QueueTrigger(QueueNames.NewStravaAthletesRequests)] Athlete athlete,
            [Table("Athletes")] CloudTable outputTable,
            [Queue(QueueNames.CollectAthleteActivities)] CloudQueue collectActivitiesQueues)
        {
            log.LogFunctionStart(FunctionsNames.Q_ProcessNewAthlete);

            var row = new AthleteEntity
            {
                PartitionKey = athlete.Id,
                RowKey = athlete.ExternalId,
                ExternalId = athlete.ExternalId,
                FirstName = athlete.FirstName,
                LastName = athlete.LastName,
                ProfilePictureUrl = athlete.ProfilePictureUrl,
                ETag = "*",
                Timestamp = DateTimeOffset.UtcNow
            };

            var operation = TableOperation.InsertOrReplace(row);
            await outputTable.ExecuteAsync(operation);   

            var input = new CollectAthleteActivitiesInput
            {
                AthleteId = row.RowKey
            };
            var json = JsonConvert.SerializeObject(input);
            await collectActivitiesQueues.AddMessageAsync(new CloudQueueMessage(json));

            log.LogFunctionEnd(FunctionsNames.Q_ProcessNewAthlete);
        }
    }

    public class AthleteEntity : TableEntity
    {
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool Active { get; set; } = true;
    }
}
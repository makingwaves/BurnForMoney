using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Queues;
using CsvHelper;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions
{
    public static class ReportGeneratorFunc
    {
        [FunctionName(FunctionsNames.T_GenerateReport)]
        public static async Task Run(
            [TimerTrigger("0 0 0 5 * *")] TimerInfo timer,
            ILogger log,
            ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.T_GenerateReport} function processed a request.");
            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            var lastMonth = DateTime.UtcNow.AddMonths(-1);

            string json;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                json = await conn
                    .QuerySingleOrDefaultAsync<string>(
                        "SELECT Results FROM dbo.[MonthlyResultsSnapshots] WHERE Date=@Date", new
                        {
                            Date = $"{lastMonth.Year}/{lastMonth.Month}"
                        })
                    .ConfigureAwait(false);
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var results = JsonConvert.DeserializeObject<List<AthleteMonthlyResult>>(json)
                .OrderBy(r => r.AthleteId)
                .Select(r =>
                {
                    r.Distance = UnitsConverter.ConvertMetersToKilometers(r.Distance);
                    r.Time = UnitsConverter.ConvertMinutesToHours(r.Time);
                    return r;
                });

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    using (var csv = new CsvWriter(streamWriter))
                    {
                        csv.Configuration.Delimiter = ";";
                        csv.WriteComment($"{lastMonth:MM/yyyy}");
                        csv.NextRecord();
                        csv.WriteRecords(results);
                        csv.Flush();
                        streamWriter.Flush();

                        var outputBlob = await GetBlobReportAsync(configuration.ConnectionStrings.AzureWebJobsStorage);
                        await outputBlob.UploadFromByteArrayAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    }
                }
            }
        }

        private static async Task<CloudBlockBlob> GetBlobReportAsync(string storageConnectionString)
        {
            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("reports");
            await container.CreateIfNotExistsAsync();

            var directory = container.GetDirectoryReference($"{lastMonth.Year}/{lastMonth.Month}/");
            return directory.GetBlockBlobReference("report.csv");
        }

        [FunctionName(FunctionsNames.B_SendNotificationWithLinkToTheReport)]
        public static async Task B_SendNotificationWithLinkToTheReport([BlobTrigger("reports")] CloudBlockBlob cloudBlob, 
            ILogger log,
            ExecutionContext executionContext,
            [Queue(AppQueueNames.NotificationsToSend)] CloudQueue notificationsQueue)
        {
            log.LogInformation($"{FunctionsNames.B_SendNotificationWithLinkToTheReport} function processed a request.");

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            var blobSasToken = GetBlobSasToken(cloudBlob, SharedAccessBlobPermissions.Read);
            log.LogInformation($"{FunctionsNames.B_SendNotificationWithLinkToTheReport} generated SAS token.");
            var link = cloudBlob.Uri + blobSasToken;

            var notification = new Notification
            {
                Recipients = new List<string> { configuration.Email.ReportsReceiver },
                Subject = "Burn for Money - Report",
                HtmlContent = $@"
Hey, <br>
A new report summarizing the previous month has been generated. You can download it from <a href={link}>this</a> address (the link is valid for 7 days)."
            };

            log.LogInformation($"{FunctionsNames.B_SendNotificationWithLinkToTheReport} created notification message.");
            var json = JsonConvert.SerializeObject(notification);
            await notificationsQueue.AddMessageAsync(new CloudQueueMessage(json));
        }

        private static string GetBlobSasToken(CloudBlob blob, SharedAccessBlobPermissions permissions)
        {
            var adHocSas = CreateAdHocSasPolicy(permissions);
            return blob.GetSharedAccessSignature(adHocSas);
        }

        private static SharedAccessBlobPolicy CreateAdHocSasPolicy(SharedAccessBlobPermissions permissions)
        {
            return new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(7),
                Permissions = permissions
            };
        }
    }
}
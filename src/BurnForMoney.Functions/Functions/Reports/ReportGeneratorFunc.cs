using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Shared.Queues;
using CsvHelper;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Reports
{
    public static class ReportGeneratorFunc
    {
        [FunctionName(FunctionsNames.T_GenerateReport)]
        public static async Task Run(
            [TimerTrigger("0 0 0 5 * *")] TimerInfo timer,
            [Configuration] ConfigurationRoot configuration,
            ILogger log)
        {
            log.LogFunctionStart(FunctionsNames.T_GenerateReport);
            var lastMonth = DateTime.UtcNow.AddMonths(-1);

            string json;
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

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

            var results = JsonConvert.DeserializeObject<List<AthleteMonthlyResult>>(json, new SingleOrArrayConverter<AthleteMonthlyResult>())
                .SelectMany(s => s.AthleteResults)
                .OrderBy(r => r.Id)
                .Select(r => new
                {
                    r.Id,
                    r.AthleteName,
                    Distance = UnitsConverter.ConvertMetersToKilometers(r.Distance).ToString(CultureInfo.InvariantCulture),
                    Time = UnitsConverter.ConvertMinutesToHours(r.Time).ToString(CultureInfo.InvariantCulture),
                    Points = r.Points.ToString(),
                    NumberOfTrainings = r.NumberOfTrainings.ToString()
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
            log.LogFunctionEnd(FunctionsNames.T_GenerateReport);
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
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.NotificationsToSend)] CloudQueue notificationsQueue)
        {
            log.LogFunctionStart(FunctionsNames.B_SendNotificationWithLinkToTheReport);

            var blobSasToken = GetBlobSasToken(cloudBlob, SharedAccessBlobPermissions.Read);
            log.LogInformation(FunctionsNames.B_SendNotificationWithLinkToTheReport, "Generated SAS token.");
            var link = cloudBlob.Uri + blobSasToken;

            var notification = new Notification
            {
                Recipients = new List<string> { configuration.Email.ReportsReceiver },
                Subject = "Burn for Money - Report",
                HtmlContent = $@"
<p>Hi there, </p>
<p>A new report summarizing the previous month has been generated. You can download it from <a href={link}>this</a> address (the link is valid for 7 days).</p>"
            };

            log.LogInformation(FunctionsNames.B_SendNotificationWithLinkToTheReport, "Created notification message.");
            var json = JsonConvert.SerializeObject(notification);
            await notificationsQueue.AddMessageAsync(new CloudQueueMessage(json));
            log.LogFunctionEnd(FunctionsNames.B_SendNotificationWithLinkToTheReport);
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using CsvHelper;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Functions.Reports
{
    public static class ReportGeneratorFunc
    {
        [FunctionName(FunctionsNames.T_GenerateReport)]
        public static async Task Run(
            [TimerTrigger("0 0 0 3 * *")] TimerInfo timer,
            [Configuration] ConfigurationRoot configuration,
            ILogger log,
            [Inject] IConnectionFactory<SqlConnection> connectionFactory)
        {
            var lastMonth = DateTime.UtcNow.AddMonths(-1);

            string json;
            using (var conn = connectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
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
                .Where(r => r.AthleteResults != null)
                .SelectMany(s => s.AthleteResults)
                .OrderBy(r => r.Id)
                .Select(r => new
                {
                    r.Id,
                    r.AthleteName,
                    Distance = UnitsConverter.ConvertMetersToKilometers(r.Distance),
                    Time = UnitsConverter.ConvertMinutesToHours(r.Time),
                    r.Points,
                    r.NumberOfTrainings
                })
                .ToList();

            if (!results.Any())
            {
                log.LogWarning("Detailed statistic from last month cannot be found.");
                return;
            }

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

                        csv.NextRecord();

                        csv.WriteComment("Sum");
                        dynamic record = new ExpandoObject();
                        record.Empty = null;
                        record.Empty = null;
                        record.Distance = results.Sum(r => r.Distance);
                        record.Time = results.Sum(r => r.Time);
                        record.Points = results.Sum(r => r.Points);
                        record.NumberOfTrainings = results.Sum(r => r.NumberOfTrainings);
                        csv.WriteRecord(record);

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
            [Configuration] ConfigurationRoot configuration,
            [Queue(AppQueueNames.NotificationsToSend)] CloudQueue notificationsQueue)
        {
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
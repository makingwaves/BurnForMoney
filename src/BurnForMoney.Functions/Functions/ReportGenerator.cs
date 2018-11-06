using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Functions.CalculateMonthlyAthleteResults;
using CsvHelper;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions
{
    public static class ReportGenerator
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

            var results = JsonConvert.DeserializeObject<List<AthleteMonthlyResult>>(json);

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
            return directory.GetBlockBlobReference("reports.csv");
        }

        [FunctionName(FunctionsNames.B_SendNotificationWithLinkToTheReport)]
        public static async Task B_SendNotificationWithLinkToTheReport([BlobTrigger("reports")] ICloudBlob cloudBlob, 
            ILogger log,
            ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.B_SendNotificationWithLinkToTheReport} function processed a request.");
        }
    }
}
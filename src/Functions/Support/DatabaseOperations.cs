using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BurnForMoney.Functions.Support
{
    public static class DatabaseOperations
    {
        private static readonly ApplicationConfiguration Configuration = new ApplicationConfiguration();

        [FunctionName("InitializeDatabase")]
        public static async Task<IActionResult> RunInitializeDatabase([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "support/intializedatabase")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("InitializeDatabase function processed a request.");

            var settings = Configuration.GetSettings(context);
            if (!settings.IsValid())
            {
                throw new Exception("Cannot read configuration file.");
            }

            using (var conn = new SqlConnection(settings.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.ExecuteAsync("CREATE TABLE dbo.[Strava.AccessTokens] ([AthleteId][int] NOT NULL, [FirstName][nvarchar](50), [LastName][nvarchar](50), [AccessToken][nvarchar](100) NOT NULL, [Active][bit] NOT NULL, PRIMARY KEY (AthleteId))")
                    .ConfigureAwait(false);

                log.Info("dbo.[Strava.AccessTokens] table created.");
            }

            return new OkObjectResult("SQL database hase been bootstrapped successfully.");
        }
    }
}
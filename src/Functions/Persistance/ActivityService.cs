using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Persistance
{
    public abstract class ActivityService
    {
        protected readonly string ConnectionString;
        protected readonly ILogger Log;

        protected ActivityService(string connectionString, ILogger log)
        {
            ConnectionString = connectionString;
            Log = log;
        }

        public async Task<UpdateHistoryState> GetState()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var result = await conn.QueryFirstOrDefaultAsync<UpdateHistoryState>("SELECT LastUpdate FROM dbo.[Systems.UpdateHistory] WHERE System='Strava'")
                    .ConfigureAwait(false);
                return result ?? new UpdateHistoryState("Strava", DateTime.UtcNow.AddMonths(-3));
            }
        }

        public async Task SaveState()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("UPDATE dbo.[Systems.UpdateHistory] SET LastUpdate=@LastUpdate WHERE System='Strava'", new
                {
                    LastUpdate = DateTime.UtcNow
                }).ConfigureAwait(false);

                if (affectedRows == 0)
                {
                    var insertedRows = await conn.ExecuteAsync("INSERT dbo.[Systems.UpdateHistory] (System, LastUpdate) VALUES (@System, @LastUpdate);", new
                    {
                        System = "Strava",
                        LastUpdate = DateTime.UtcNow
                    }).ConfigureAwait(false);
                    if (insertedRows != 1)
                    {
                        Log.LogError("[Strava] State cannot be saved.");
                        return;
                    }
                }

                Log.LogInformation("[Strava] Updated state.");
            }
        }

        public class UpdateHistoryState
        {
            public string System { get; set; }
            public DateTime LastUpdate { get; set; }

            public UpdateHistoryState(string system, DateTime lastUpdate)
            {
                System = system;
                LastUpdate = lastUpdate;
            }

            public UpdateHistoryState()
            {

            }
        }
    }
}
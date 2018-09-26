using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Persistance;
using BurnForMoney.Functions.Strava.Model;
using Dapper;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Services
{
    public class StravaActivityService : ActivityService
    {
        public StravaActivityService(string connectionString, ILogger log) : base(connectionString, log)
        {
        }

        public async Task InsertAsync(Activity activity)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync("Strava_Activity_Insert",
                        new
                        {
                            AthleteId = activity.Athlete.Id,
                            ActivityId = activity.Id,
                            ActivityTime = activity.StartDate,
                            ActivityType = activity.Type.ToString(),
                            Distance = activity.Distance,
                            MovingTime = activity.MovingTime
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    Log.LogInformation($"Activity with id: {activity.Id} has been added.");
                }
            }
        }
    }
}
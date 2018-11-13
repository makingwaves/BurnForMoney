using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public static class DeleteActivityFunc
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log, ExecutionContext executionContext,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] long activityId)
        {
            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                var affectedRows = await conn.ExecuteAsync(@"DELETE FROM dbo.Activities WHERE ActivityId=@ActivityId", new { ActivityId = activityId });
                if (affectedRows == 0)
                {
                    throw new FailedToDeleteActivityException(activityId.ToString());
                }
            }
        }
    }
}
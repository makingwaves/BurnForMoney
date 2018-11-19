using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Functions.Shared.Queues;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public static class DeleteActivityFunc
    {
        [FunctionName(FunctionsNames.Q_DeleteActivity)]
        public static async Task Q_DeleteActivity(ILogger log,
            [QueueTrigger(AppQueueNames.DeleteActivityRequests)] DeleteActivityRequest deleteRequest)
        {
            log.LogFunctionStart(FunctionsNames.Q_DeleteActivity);
            var configuration = ApplicationConfiguration.GetSettings();
            using (var conn = SqlConnectionFactory.Create(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                await conn.OpenWithRetryAsync();

                if (!string.IsNullOrWhiteSpace(deleteRequest.Id))
                {
                    var affectedRows = await conn.ExecuteAsync(@"DELETE FROM dbo.Activities WHERE Id=@Id", new { deleteRequest.Id });
                    if (affectedRows == 0)
                    {
                        throw new FailedToDeleteActivityException(deleteRequest.Id);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(deleteRequest.ExternalId))
                {
                    var affectedRows = await conn.ExecuteAsync(@"DELETE FROM dbo.Activities WHERE ExternalId=@ExternalId", new { deleteRequest.ExternalId });
                    if (affectedRows == 0)
                    {
                        throw new FailedToDeleteActivityException(deleteRequest.ExternalId);
                    }
                }

            }
            log.LogFunctionEnd(FunctionsNames.Q_DeleteActivity);
        }
    }
}
using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace BurnForMoney.Functions.Shared.Persistence
{
    public class SqlConnectionFactory
    {
        public static IDbConnection Create(string connectionString)
        {
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
            var retryPolicy =
                new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategy);

            return new ReliableSqlConnection(connectionString, retryPolicy);
        }
    }
}
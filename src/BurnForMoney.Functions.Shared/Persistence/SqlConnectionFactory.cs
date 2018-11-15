using System.Data;
using System.Data.SqlClient;

namespace BurnForMoney.Functions.Shared.Persistence
{
    public class SqlConnectionFactory
    {
        public const int ConnectRetryCount = 6;
        public const int ConnectRetryInterval = 5;
        public const int ConnectionTimeout = 30;

        public static IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public static IDbConnection CreateWithRetry(string connectionString)
        {
            connectionString += $";ConnectRetryCount={ConnectRetryCount};ConnectRetryInterval={ConnectRetryInterval};Connection Timeout={ConnectionTimeout};";

            return Create(connectionString);
        }
    }
}
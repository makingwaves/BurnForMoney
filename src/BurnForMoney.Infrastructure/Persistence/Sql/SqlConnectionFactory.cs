using System.Data.SqlClient;

namespace BurnForMoney.Infrastructure.Persistence.Sql
{
    public class SqlConnectionFactory
    {
        public const int ConnectRetryCount = 6;
        public const int ConnectRetryInterval = 5;
        public const int ConnectionTimeout = 30;

        public static SqlConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
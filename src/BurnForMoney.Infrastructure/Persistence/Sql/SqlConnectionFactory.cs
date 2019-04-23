using System.Data;
using System.Data.SqlClient;

namespace BurnForMoney.Infrastructure.Persistence.Sql
{
    public interface IConnectionFactory<out T> where T : IDbConnection
    {
        T Create(string connectionString);
    }

    public class SqlConnectionFactory : IConnectionFactory<SqlConnection>
    {
        public const int ConnectRetryCount = 6;
        public const int ConnectRetryInterval = 5;
        public const int ConnectionTimeout = 30;

        public static SqlConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        SqlConnection IConnectionFactory<SqlConnection>.Create(string connectionString)
        {
            return Create(connectionString);
        }
    }
}
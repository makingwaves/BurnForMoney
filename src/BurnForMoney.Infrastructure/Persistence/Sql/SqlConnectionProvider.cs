using System.Data;
using System.Data.SqlClient;

namespace BurnForMoney.Infrastructure.Persistence.Sql
{
    public interface IConnectionProvider<out T> where T : IDbConnection
    {
        T Create();
    }

    public class SqlConnectionProvider : IConnectionProvider<SqlConnection>
    {
        private readonly string _connectionString;

        public SqlConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Serilog;

namespace BurnForMoney.RegenerateViews
{
    public class TableWiper
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public TableWiper(string connectionString, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = connectionString;
        }

        public void Wipe(IEnumerable<string> tables)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                _logger.Information($"Wiping tables in database: {connection.Database}");
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        try
                        {
                            command.Transaction = transaction;
                            TruncateTables(tables, command, transaction);
                            transaction.Commit();
                        }
                        finally
                        {
                            _logger.Information("Wiping finished");
                        }
                    }
                }
            }
        }

        private void TruncateTables(IEnumerable<string> tables, IDbCommand command, IDbTransaction transaction)
        {
            foreach (string table in tables)
            {
                try
                {
                    _logger.Verbose($"Truncating table: {table}");
                    command.CommandText = $"DELETE FROM [{table}]";
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception occured when trying to truncate table: {table}. Details: {ex}");
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BurnForMoney.Functions.Presentation.Exceptions;
using BurnForMoney.Infrastructure.Events;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace BurnForMoney.Functions.Presentation.Functions.ResultsSnapshots.MonthlyResultsUpdatesStrategies
{
    public class MonthlyResultsChangeRequest
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid AthleteId { get; set; }
        public string Category { get; set; }
        public int Distance { get; set; }
        public int MovingTime { get; set; }
        public int Points { get; set; }
        public PreviousActivityData PreviousData { get; set; }
    }

    public abstract class MonthlyResultsChangeStrategy
    {
        private readonly string _sqlConnectionString;

        protected MonthlyResultsChangeStrategy(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        protected abstract void UpdateResult(AthleteMonthlyResult result, MonthlyResultsChangeRequest request);

        public async Task CreateOrUpdateResults(MonthlyResultsChangeRequest request)
        {
            using (SqlConnection connection = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await connection.OpenWithRetryAsync();
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string date = $"{request.Year}/{request.Month}";
                        AthleteMonthlyResult result = await GetCurrentSnapshot(connection, date, transaction);
                        PerformInitialUpdate(request, result);
                        UpdateResult(result, request);

                        string json = JsonConvert.SerializeObject(result);
                        int affectedRows = await ExecuteUpsert(connection, date, json, transaction);

                        if (affectedRows == 0)
                        {
                            throw new SqlConcurrencyException();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static async Task<int> ExecuteUpsert(IDbConnection connection, string date, string json,
            SqlTransaction transaction)
        {
            int affectedRows = await connection.ExecuteAsync("MonthlyResultsSnapshots_Upsert", new
                {
                    Date = date,
                    Results = json
                }, transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
            return affectedRows;
        }

        private static void PerformInitialUpdate(MonthlyResultsChangeRequest request, AthleteMonthlyResult result)
        {
            result.Points += request.Points;
            result.Distance += request.Distance;
            result.Time += request.MovingTime;
        }

        protected static void UpdateAthleteResultMetrics(MonthlyResultsChangeRequest request,
            AthleteResult athleteResult)
        {
            athleteResult.Points += request.Points;
            athleteResult.Distance += request.Distance;
            athleteResult.Time += request.MovingTime;
        }

        protected static void UpdateActivityMetrics(MonthlyResultsChangeRequest request,
            AthleteMonthlyResultActivity activity)
        {
            activity.Points += request.Points;
            activity.Distance += request.Distance;
            activity.Time += request.MovingTime;
        }

        private static async Task<AthleteMonthlyResult> GetCurrentSnapshot(IDbConnection connection, string date,
            IDbTransaction transaction)
        {
            string snapshot = await connection.QuerySingleOrDefaultAsync<string>(
                @"SELECT Results FROM MonthlyResultsSnapshots WITH (TABLOCKX, HOLDLOCK) WHERE Date = @Date;",
                new {Date = date},
                transaction
            ).ConfigureAwait(false);
            AthleteMonthlyResult result = MapToAthleteMonthlyResults(snapshot);
            return result;
        }

        private static AthleteMonthlyResult MapToAthleteMonthlyResults(string snapshot)
        {
            return string.IsNullOrEmpty(snapshot)
                ? AthleteMonthlyResult.NoResults
                : JsonConvert.DeserializeObject<AthleteMonthlyResult>(snapshot);
        }
    }
}
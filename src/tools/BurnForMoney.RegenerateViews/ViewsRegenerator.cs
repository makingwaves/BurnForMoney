using BurnForMoney.Domain;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Infrastructure.Persistence;
using Newtonsoft.Json;
using Serilog.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;
using ILogger = Serilog.ILogger;
using MelLogger = Microsoft.Extensions.Logging.ILogger;

namespace BurnForMoney.RegenerateViews
{
    public class ViewsRegenerator
    {
        private readonly Options _options;
        private readonly ILogger _logger;

        public ViewsRegenerator(Options options, ILogger logger)
        {
            _options = options;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void TestConnection()
        {
            EventStore.Create(_options.AzureStorageConnectionString, null);
        }

        public void Regenerate()
        {
            RestoreArchivalData();

            var domainEventsDispatcher = CreateDispatcher(_options, _logger);
            var eventStore = (EventStore) EventStore.Create(_options.AzureStorageConnectionString, null);
            
            _logger.Verbose("Listing all aggregates from azure store.");
            var aggregatesIds = eventStore.ListAggregates().Result;

            _logger.Information($"{aggregatesIds.Count} aggregates listed.");
            _logger.Information("Reprocessing events for aggregates.");

            foreach (var aggregateId in aggregatesIds)
            {
                _logger.Verbose($"Reading events for aggregate: '{aggregateId}'");
                var events = eventStore.GetEventsForAggregateAsync(aggregateId).Result.OrderBy(de => de.Version).ToList();
                _logger.Verbose($"{events.Count} events read for aggregate: '{aggregateId}'");

                foreach (var @event in events)
                {
                    _logger.Verbose(
                        $"Apply event '{@event.GetType().Name}' (ver:{@event.Version}) for aggregate '{aggregateId}'");

                    if (_options.ExtraVerbose)
                        _logger.LogEventDetails(@event);

                SafeDispatch(domainEventsDispatcher, @event, _logger);
                }
            }
        }

        private void RestoreArchivalData()
        {
            _logger.Information("Restoring archival data.");
            using (var conn = SqlConnectionFactory.Create(_options.MsSqlConnectionString))
            {
                conn.Open();

                try
                {
                    foreach (var data in ViewsRegeneratorArchivalData.ArchivalData)
                    {
                        var json = JsonConvert.SerializeObject(data.Results);
                        var affectedRows = conn.Execute("MonthlyResultsSnapshots_Upsert", new
                        {
                            data.Date,
                            Results = json
                        }, commandType: CommandType.StoredProcedure);

                        if (affectedRows == 1)
                            _logger.Verbose($"Archival data for month: {data.Date} have been added.");
                        else
                            _logger.Error($"Archival data for month: {data.Date} NOT added.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"An error occured. {ex.Message}.");
                    throw;
                }

            }
        }

        private PresentationEventsDispatcher CreateDispatcher(Options options, ILogger logger)
        {
            var melLogger = new SerilogLoggerProvider(logger).CreateLogger(nameof(Program));
            var domainEventsDispatcher = new PresentationEventsDispatcher(options.MsSqlConnectionString, melLogger);
            return domainEventsDispatcher;
        }

        private static void SafeDispatch(PresentationEventsDispatcher dispatcher, DomainEvent @event, ILogger logger)
        {
            try
            {
                dispatcher.DispatchAthleteEvent(@event).Wait();
            }
            catch (Exception e)
            {
                logger.LogErrorDetails(e, @event);
            }

            try
            {
                dispatcher.DispatchActivityEvent(@event).Wait();
            }
            catch (Exception e)
            {
                logger.LogErrorDetails(e, @event);
            }
        }
    }

    internal static class LoggerExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static void LogErrorDetails(this ILogger logger, Exception e, DomainEvent @event)
        {
            var eventString = CreateEventString(@event);
            logger.Error($"An exception occurred while processing event '{@event.GetType().FullName}':" +
                         $"{Environment.NewLine}{eventString}{Environment.NewLine}Exception: {e}");
        }

        public static void LogEventDetails(this ILogger logger, DomainEvent @event)
        {
            var eventString = CreateEventString(@event);
            logger.Verbose($"Event details: {eventString}");
        }

        private static string CreateEventString(DomainEvent @event)
        {
            return JsonConvert.SerializeObject(@event, SerializerSettings);
        }
    }
}
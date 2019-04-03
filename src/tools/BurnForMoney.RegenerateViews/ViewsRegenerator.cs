using System;
using System.Collections.Generic;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Infrastructure.Persistence;
using Newtonsoft.Json;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;
using MelLogger = Microsoft.Extensions.Logging.ILogger;

namespace BurnForMoney.RegenerateViews
{
    public class ViewsRegenerator
    {
        private readonly ILogger _logger;

        public ViewsRegenerator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Regenerate(Options options)
        {
            PresentationEventsDispatcher domainEventsDispatcher = CreateDispatcher(options, _logger);
            var eventStore = (EventStore) EventStore.Create(options.AzureStorageConnectionString, null);

            _logger.Verbose("Listing all aggregates from azure store.");
            List<Guid> aggregatesIds = eventStore.ListAggregates().Result;

            _logger.Information($"{aggregatesIds.Count} aggregates listed.");
            _logger.Information("Reprocessing events for aggregates.");

            foreach (Guid aggregateId in aggregatesIds)
            {
                _logger.Verbose($"Reading events for aggregate: '{aggregateId}'");
                List<DomainEvent> events = eventStore.GetEventsForAggregateAsync(aggregateId).Result;
                _logger.Verbose($"{events.Count} events read for aggregate: '{aggregateId}'");

                foreach (DomainEvent @event in events)
                {
                    _logger.Verbose($"Apply event '{@event.GetType().Name}' for aggregate '{aggregateId}'");
                    if (options.ExtraVerbose) _logger.LogEventDetails(@event);
                    SafeDispatch(domainEventsDispatcher, @event, _logger);
                }
            }
        }

        private PresentationEventsDispatcher CreateDispatcher(Options options, ILogger logger)
        {
            MelLogger melLogger = new SerilogLoggerProvider(logger).CreateLogger(nameof(Program));
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
            string eventString = CreateEventString(@event);
            logger.Error($"An exception occurred while processing event '{@event.GetType().FullName}':" +
                         $"{Environment.NewLine}{eventString}{Environment.NewLine}Exception: {e}");
        }

        public static void LogEventDetails(this ILogger logger, DomainEvent @event)
        {
            string eventString = CreateEventString(@event);
            logger.Verbose($"Event details: {eventString}");
        }

        private static string CreateEventString(DomainEvent @event)
        {
            return JsonConvert.SerializeObject(@event, SerializerSettings);
        }
    }
}
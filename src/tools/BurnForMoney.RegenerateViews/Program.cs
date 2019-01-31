using System;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Infrastructure.Persistence;
using CommandLine;
using Newtonsoft.Json;
using Serilog;

namespace BurnForMoney.RegenerateViews
{
    public static partial class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options => {
                var configuration = new LoggerConfiguration();
                
                if(!options.Silent)
                    configuration.WriteTo.Console();
                
                if(!string.IsNullOrEmpty(options.LogFileName))
                     configuration.WriteTo.File(options.LogFileName);

                if(options.Verbose || options.ExtraVerbose)
                    configuration.MinimumLevel.Verbose();
                
                using(var logger = configuration.CreateLogger())
                {
                    try
                    {
                        //ClearView(options, logger);
                        RegenerateViews(options, logger);
                    }catch(Exception e)
                    {
                        logger.Error(e.ToString());
                        throw;
                    }
                }
            });
        }

        private static void ClearView(Options options, ILogger logger)
        {
            throw new NotImplementedException();
        }

        private static void RegenerateViews(Options options, ILogger logger)
        {
            logger.Information("Starting regenerate BFM views.");
            logger.Verbose("Verbose logging level enabled.");
            
            logger.Verbose($"Azure Store connection String: '{options.AzureStorageConnectionString}'");
            logger.Verbose($"MsSql connection string: '{options.MsSqlConnectionString}'");
            
            if(!string.IsNullOrEmpty(options.LogFileName))
                logger.Verbose($"Log output file: '{options.LogFileName}'");

            var domainEventsDispacher = new PresentationEventsDispatcher(options.MsSqlConnectionString);
            var  eventStore = (EventStore)EventStore.Create(options.AzureStorageConnectionString, null);

            logger.Verbose($"Listing all aggregates from azure store.");
            var aggregatesIds = eventStore.ListAggregates().Result;

            logger.Information($"{aggregatesIds.Count} aggregates listed.");
            logger.Information("Reprocessing events for aggregates.");

            foreach(var aggregateId in aggregatesIds)
            {
                logger.Verbose($"Reading events for aggregate: '{aggregateId}'");
                var events = eventStore.GetEventsForAggregateAsync(aggregateId).Result;
                logger.Verbose($"{events.Count} events read for aggregate: '{aggregateId}'");
                
                foreach(var @event in events)
                {
                    logger.Verbose($"Apply event '{@event.GetType().Name}' for aggregate '{aggregateId}'");
                    if(options.ExtraVerbose)
                        logger.LogEventDetails(@event);
                    domainEventsDispacher.SafeDispatch(@event, logger);
                }
            }
        }

        private static void SafeDispatch(this PresentationEventsDispatcher dispatcher,  DomainEvent @event, ILogger logger)
        {
            try
            {
                dispatcher.DispatchAthleteEvent(@event).Wait();
            }
            catch(Exception e)
            {
                logger.LogErrorDetails(e, @event);
            }

            try
            {
                dispatcher.DispatchActivityEvent(@event).Wait();
            }
            catch(Exception e)
            {
                logger.LogErrorDetails(e, @event);
            }
        }

        private static void LogErrorDetails(this ILogger logger, Exception e, DomainEvent @event)
        {
            var eventString = JsonConvert.SerializeObject(@event, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
            logger.Error($"An exception occurred while processing event '{@event.GetType().FullName}':{Environment.NewLine}{eventString}{Environment.NewLine}Exception: {e}");
        }

        private static void LogEventDetails(this ILogger logger, DomainEvent @event)
        {
            var eventString = JsonConvert.SerializeObject(@event, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
            logger.Verbose($"Event details: {eventString}");
        }
    }
}

﻿using System;
using System.Collections.Generic;
using CommandLine;
using Serilog;
using Serilog.Core;
using ILogger = Serilog.ILogger;

namespace BurnForMoney.RegenerateViews
{
    public static class Program
    {
        //order is important due to key constraints!
        private static readonly string[] Tables =
        {
            "Activities",
            "IndividualResults",
            "Athletes"
        };

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseErrors);
        }

        private static void Run(Options options)
        {
            LoggerConfiguration configuration = CreateLoggerConfiguration(options);
            using (Logger logger = configuration.CreateLogger())
            {
                try
                {
                    LogExecutionInfo(logger, options);
                    ClearDatabase(options, logger);
                    RegenerateViews(options, logger);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw;
                }
                finally
                {
                    logger.Information("Finished");
                }
            }
        }

        private static void HandleParseErrors(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
            {
                Console.WriteLine(error);
            }
        }

        private static LoggerConfiguration CreateLoggerConfiguration(Options options)
        {
            var configuration = new LoggerConfiguration();

            if (!options.Silent)
                configuration.WriteTo.Console();

            if (!string.IsNullOrEmpty(options.LogFileName))
                configuration.WriteTo.File(options.LogFileName);

            if (options.Verbose || options.ExtraVerbose)
                configuration.MinimumLevel.Verbose();

            return configuration;
        }

        private static void LogExecutionInfo(ILogger logger, Options options)
        {
            logger.Information("Starting regenerate BFM views.");
            logger.Verbose("Verbose logging level enabled.");

            logger.Verbose($"Azure Store connection String: '{options.AzureStorageConnectionString}'");
            logger.Verbose($"MsSql connection string: '{options.MsSqlConnectionString}'");

            if (!string.IsNullOrEmpty(options.LogFileName))
                logger.Verbose($"Log output file: '{options.LogFileName}'");
        }

        private static void ClearDatabase(Options options, ILogger logger)
        {
            var wiper = new TableWiper(options.MsSqlConnectionString, logger);
            wiper.Wipe(Tables);
        }

        private static void RegenerateViews(Options options, ILogger logger)
        {
            var regenerator = new ViewsRegenerator(logger);
            regenerator.Regenerate(options);
        }
    }
}
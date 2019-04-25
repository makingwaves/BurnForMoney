using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Microsoft.VisualBasic.FileIO;
using RestSharp;
using Serilog;

namespace BurnForMoney.ActiveDirectoryIdMigration
{
    public class Options
    {
        [Option('a', "api_url", Required = true, HelpText = "BFM Internal Api Url.")]
        public string InternalApiUrl { get; set; }

        [Option('c', "code", Required = true, HelpText = "BFM Internal Api master code.")]
        public string InternalApiMasterCode { get; set; }

        [Option('m', "map", Required = true, HelpText = "CSV file with athlete id -> aadId mapping.")]
        public string AadIdCsvMap { get; set; }

        [Option('l', "log", Required = false, HelpText = "Output log file name.", Default = null)]
        public string LogFileName { get; set; }
    }
    
    static class Program
    {
        /*
         * TODO - THIS PROGRAM IS NO FINISHED !
         */

        static void Main(string[] args)
        {
            var optionas = new Options
            {
                LogFileName = "log.txt",
                AadIdCsvMap = @"C:\Users\Daniel.Domurad\Desktop\aadid_map.csv",
                InternalApiMasterCode = "code",
                InternalApiUrl = "uri :)"
            };

            Run(optionas);
        }

        private static void Run(Options options)
        {
            var configuration = CreateLoggerConfiguration(options);
            using (var logger = configuration.CreateLogger())
            {
                var idMap = ReadAthelteAadIdMapping(options);
                foreach (var mapping in idMap)
                {
                    try
                    {
                        Migrate(mapping.Key, mapping.Value, logger, options);
                    }
                    catch (Exception e)
                    {
                        logger.Error($"Migration failed for: {mapping.Key}", e);
                        throw;
                    }
                }
            }
        }

        private static LoggerConfiguration CreateLoggerConfiguration(Options options)
        {
            var configuration = new LoggerConfiguration();
            configuration.WriteTo.Console();

            if (!string.IsNullOrEmpty(options.LogFileName))
                configuration.WriteTo.File(options.LogFileName);
            
            return configuration;
        }

        private static void Migrate(Guid athleteId, Guid aadId, ILogger logger, Options options)
        {
            logger.Information($"Migrating '{athleteId}' (aadId={aadId}");

            var response = new RestClient(options.InternalApiUrl)
                .ExecuteAsPost(new RestRequest(), 
                    $"/api/athlete/asign_aad?code={options.InternalApiMasterCode}");

            if(!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        private static Dictionary<Guid, Guid> ReadAthelteAadIdMapping(Options options)
        {
            var output = new Dictionary<Guid, Guid>();
            
            using (var parser = new TextFieldParser(options.AadIdCsvMap))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                var header = parser.ReadFields();

                if (header.Count() < 2)
                    throw new Exception("header.Count() < 2");

                if(header[0] != "id" || header[1] != "aadId")
                    throw new Exception("header[0] != \"id\" || header[1] != \"aadId\"");

                while (!parser.EndOfData)
                {
                    var row = parser.ReadFields();
                    output.Add(Guid.Parse(row[0]), Guid.Parse(row[1]));
                }
            }

            return output;
        }
    }
}

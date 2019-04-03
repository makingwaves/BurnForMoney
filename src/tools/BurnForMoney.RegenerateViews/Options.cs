using CommandLine;

namespace BurnForMoney.RegenerateViews
{
    public class Options
    {   
        [Option('i', "input_storage", Required = true, HelpText = "Azure storage connection string.")]
        public string AzureStorageConnectionString {get; set;}
    
        [Option('o', "output_storage", Required = true, HelpText = "MS-SQL connection string.")]
        public string MsSqlConnectionString {get; set;}

        [Option("verbose", Required = false, HelpText = "Set output level to verbose.", Default = false)]
        public bool Verbose {get; set;}

        [Option("extra_verbose", Required = false, HelpText = "Set output level to extra verbose. This will serialize and log all events.", Default = false)]
        public bool ExtraVerbose {get; set;}

        [Option('l', "log", Required = false, HelpText = "Output log file name.", Default = null)]
        public string LogFileName {get; set;}

        [Option('s', "silent", Required = false, HelpText = "Disable console logging.", Default = false)]
        public bool Silent {get; set;}
    }
}
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace BurnForMoney.Functions.Configuration
{
    public class ApplicationConfiguration
    {
        public ConfigurationRoot GetSettings(ExecutionContext context)
        {
            var config = GetApplicationConfiguration(context.FunctionAppDirectory);
            var sqlConnectionString = config.GetConnectionString("SQLConnectionString");

            return new ConfigurationRoot
            {
                Strava = GetStravaConfiguration(config),
                SqlDbConnectionString = sqlConnectionString
            };
        }

        private static StravaConfigurationSection GetStravaConfiguration(IConfigurationRoot config)
        {
            int.TryParse(config["Strava.ClientId"], out var clientId);
            var clientSecret = config["Strava.ClientSecret"];
            return new StravaConfigurationSection(clientId, clientSecret);
        }

        private static IConfigurationRoot GetApplicationConfiguration(string functionAppDirectory)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(functionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return config;
        }
    }
}
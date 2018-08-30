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
                Strava = GetStravaSection(config),
                SqlDbConnectionString = sqlConnectionString
            };
        }

        private static StravaConfigurationSection GetStravaSection(IConfigurationRoot config)
        {
            var stravaConfig = config.GetSection("Strava");
            int.TryParse(stravaConfig["ClientId"], out var clientId);
            var clientSecret = stravaConfig["ClientSecret"];
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
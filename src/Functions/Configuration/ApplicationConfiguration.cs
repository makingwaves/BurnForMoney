using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace BurnForMoney.Functions.Configuration
{
    public class ApplicationConfiguration
    {
        private static ConfigurationRoot _settings;

        public static ConfigurationRoot GetSettings(ExecutionContext context)
        {
            if (_settings == null)
            {
                var config = GetApplicationConfiguration(context.FunctionAppDirectory);

                _settings = new ConfigurationRoot
                {
                    Strava = GetStravaConfiguration(config),
                    ConnectionStrings = GetConnectionStrings(config),
                    IsLocalEnvironment = string.IsNullOrEmpty(GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId))
                };

                if (!_settings.IsValid())
                {
                    throw new Exception("Cannot read configuration file.");
                }
            }

            return _settings;
        }

        private static ConnectionStringsSection GetConnectionStrings(IConfigurationRoot config)
        {
            return new ConnectionStringsSection
            {
                SqlDbConnectionString = config.GetConnectionString("SQL.ConnectionString"),
                KeyVaultConnectionString = config.GetConnectionString("KeyVault.ConnectionString"),
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

        public static string GetEnvironmentVariable(string settingKey)
        {
            string settingValue = null;
            if (!string.IsNullOrEmpty(settingKey))
            {
                settingValue = Environment.GetEnvironmentVariable(settingKey);
            }

            return settingValue;
        }
}
}
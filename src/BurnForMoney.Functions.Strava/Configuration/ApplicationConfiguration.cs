using System;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.Extensions.Configuration;

namespace BurnForMoney.Functions.Strava.Configuration
{
    public class ApplicationConfiguration
    {
        private static ConfigurationRoot _settings;

        internal static ConfigurationRoot GetSettings()
        {
            return GetSettings(Environment.CurrentDirectory);
        }

        public static ConfigurationRoot GetSettings(string functionAppDirectory)
        {
            if (_settings == null)
            {
                var config = GetApplicationConfiguration(functionAppDirectory);

                var isLocal = string.IsNullOrEmpty(GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId));
                _settings = new ConfigurationRoot
                {
                    IsLocalEnvironment = isLocal,
                    ConnectionStrings = new ConnectionStringsSection
                    {
                        SqlDbConnectionString = config["ConnectionStrings:Sql"],
                        AzureWebJobsStorage = config["AzureWebJobsStorage"]
                    },
                    Strava = new StravaConfigurationSection
                    {
                        ClientId = int.Parse(config["Strava:ClientId"]),
                        ClientSecret = config["Strava:ClientSecret"],
                        AccessTokensEncryptionKey = config["strava:AccessTokensEncryptionKey"]
                    },
                    Email = config.Get<EmailSection>("Email"),
                    HostName = config["WEB_HOST"],
                    ApplicationInsightsInstrumentationKey = config["APPINSIGHTS_INSTRUMENTATIONKEY"]
                };

                if (!_settings.IsValid())
                {
                    throw new Exception("Cannot read configuration file.");
                }
            }

            return _settings;
        }

        private static IConfigurationRoot GetApplicationConfiguration(string functionAppDirectory)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(functionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var builtConfig = config.Build();
            config.AddAzureKeyVault($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/");

            return config.Build();
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
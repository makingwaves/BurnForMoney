using System;
using Microsoft.Extensions.Configuration;

namespace BurnForMoney.Functions.PublicApi.Configuration
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

                _settings = new ConfigurationRoot
                {
                    ConnectionStrings = new ConnectionStringsSection
                    {
                        SqlDbConnectionString = config["ConnectionStrings:Sql"]
                    },
                    CompanyInformation = config.GetSection("CompanyInformation").Get<CompanyInformationSection>()
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
    }
}
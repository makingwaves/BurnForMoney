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

                var isLocal = string.IsNullOrEmpty(GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId));
                _settings = new ConfigurationRoot
                {
                    IsLocalEnvironment = isLocal,
                    ConnectionStrings = new ConnectionStringsSection
                    {
                        SqlDbConnectionString = config["ConnectionStrings:Sql"],
                        AzureWebJobsStorage = config["AzureWebJobsStorage"]
                    },
                    Email = new EmailSection
                    {
                        AthletesApprovalEmail = config["Email:AthletesApprovalEmail"],
                        SenderEmail = "burnformoney@makingwaves.com",
                        DefaultRecipient = config["Email:DefaultRecipient"]
                    },
                    StravaAppHostName = config["StravaAppHostName"],
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
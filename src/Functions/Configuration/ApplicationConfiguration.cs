using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace BurnForMoney.Functions.Configuration
{
    public class ApplicationConfiguration
    {
        private const string LocalHostName = "0.0.0.0";
        private static ConfigurationRoot _settings;
        private static readonly IKeyVaultClient KeyVaultClient = KeyVaultClientFactory.Create();

        public static async Task<ConfigurationRoot> GetSettingsAsync(ExecutionContext context)
        {
            if (_settings == null)
            {
                var config = GetApplicationConfiguration(context.FunctionAppDirectory);
                var keyVaultConnectionString = config.GetConnectionString("KeyVault.ConnectionString");

                var isLocal = string.IsNullOrEmpty(GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId));
                _settings = new ConfigurationRoot
                {
                    IsLocalEnvironment = isLocal,
                    ConnectionStrings = new ConnectionStringsSection
                    {
                        KeyVaultConnectionString = keyVaultConnectionString,
                        SqlDbConnectionString = isLocal ? "Data Source=(LocalDB)\\.;Initial Catalog=BurnForMoney;Integrated Security=True" :
                        await GetKeyVaultSecretAsync(keyVaultConnectionString, KeyVaultSecretNames.SqlConnectionString),
                        AzureWebJobsStorage = config["AzureWebJobsStorage"]
                    },
                    Strava = new StravaConfigurationSection
                    {
                        ClientId = int.Parse(config["Strava.ClientId"]),
                        ClientSecret = config["Strava.ClientSecret"]
                    },
                    Email = new EmailSection
                    {
                        SendGridApiKey = config["SendGrid.ApiKey"],
                        AthletesApprovalEmail = config["Email.AthletesApprovalEmail"],
                        SenderEmail = "burnformoney@makingwaves.com"
                    },
                    HostName = GetHostName()
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

        private static string GetHostName()
        {
            var hostName = Environment.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteHostName);

            if (hostName == null)
            {
                throw new ArgumentNullException(EnvironmentSettingNames.AzureWebsiteHostName);
            }

            if (hostName.StartsWith(LocalHostName))
            {
                hostName = hostName.Replace(LocalHostName, "localhost");
            }

            return hostName;
        }

        private static async Task<string> GetKeyVaultSecretAsync(string keyVaultConnectionString, string secretName)
        {
            var secret = await KeyVaultClient.GetSecretAsync(keyVaultConnectionString, secretName)
                .ConfigureAwait(false);
            return secret.Value;
        }
    }
}
using System;
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
        private static readonly IKeyVaultClient _keyVaultClient = KeyVaultClientFactory.Create();

        public static async Task<ConfigurationRoot> GetSettingsAsync(ExecutionContext context)
        {
            if (_settings == null)
            {
                var config = GetApplicationConfiguration(context.FunctionAppDirectory);

                var isLocal = string.IsNullOrEmpty(GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId));
                _settings = new ConfigurationRoot
                {
                    IsLocalEnvironment = isLocal,
                    ConnectionStrings = await GetConnectionStringsAsync(config, isLocal),
                    Strava = GetStravaConfiguration(config),
                    Email = GetEmailConfiguration(config),
                    HostName = GetHostName()
                };

                if (!_settings.IsValid())
                {
                    throw new Exception("Cannot read configuration file.");
                }
            }

            return _settings;
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

        private static EmailSection GetEmailConfiguration(IConfigurationRoot config)
        {
            return new EmailSection
            {
                SendGridApiKey = config["SendGrid.ApiKey"],
                AthletesApprovalEmail = config["Email.AthletesApprovalEmail"],
                SenderEmail = "burnformoney@makingwaves.com"
            };
        }

        private static async Task<ConnectionStringsSection> GetConnectionStringsAsync(IConfigurationRoot config, bool isLocal)
        {
            var keyVaultConnectionString = config.GetConnectionString("KeyVault.ConnectionString");
            var sqlDbConnectionString = isLocal ? "Data Source=(LocalDB)\\.;Initial Catalog=BurnForMoney;Integrated Security=True" :
                await GetSecretFromKeyVaultAsync(keyVaultConnectionString, KeyVaultSecretNames.SQLConnectionString);

            return new ConnectionStringsSection
            {
                KeyVaultConnectionString = keyVaultConnectionString,
                SqlDbConnectionString = sqlDbConnectionString
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

        private static async Task<string> GetSecretFromKeyVaultAsync(string keyVaultConnectionString, string secretName)
        {
            var secret = await _keyVaultClient.GetSecretAsync(keyVaultConnectionString, secretName)
                .ConfigureAwait(false);
            return secret.Value;
        }
    }
}
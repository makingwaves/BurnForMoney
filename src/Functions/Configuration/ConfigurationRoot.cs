namespace BurnForMoney.Functions.Configuration
{
    public class ConfigurationRoot
    {
        public StravaConfigurationSection Strava { get; set; }
        public EmailSection Email { get; set; }
        public ConnectionStringsSection ConnectionStrings { get; set; }
        public bool IsLocalEnvironment { get; set; }
        public string HostName { get; set; }

        public bool IsValid()
        {
            return Strava != null && ConnectionStrings != null;
        }
    }

    public class EmailSection
    {
        public string SendGridApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string MainRecipientEmail { get; set; }
    }

    public class ConnectionStringsSection
    {
        public string SqlDbConnectionString { get; set; }
        public string KeyVaultConnectionString { get; set; }
    }

    public class StravaConfigurationSection
    {
        public int ClientId { get; set; }
        public string ClientSecret { get; set; }

        public StravaConfigurationSection(int clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}
namespace BurnForMoney.Functions.Strava.Configuration
{
    public class ConfigurationRoot
    {
        public StravaConfigurationSection Strava { get; set; }
        public EmailSection Email { get; set; }
        public ConnectionStringsSection ConnectionStrings { get; set; }
        public bool IsLocalEnvironment { get; set; }
        public string HostName { get; set; }
        public string ApplicationInsightsInstrumentationKey { get; set; }

        public bool IsValid()
        {
            return Strava != null && ConnectionStrings != null;
        }
    }

    public class EmailSection
    {
        public string AthletesApprovalEmail { get; set; }
        public string DefaultRecipient { get; set; }
    }

    public class ConnectionStringsSection
    {
        public string SqlDbConnectionString { get; set; }
        public string AzureWebJobsStorage { get; set; }
    }

    public class StravaConfigurationSection
    {
        public int ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessTokensEncryptionKey { get; set; }
        public string ConfirmationPageUrl { get; set; }
    }
}
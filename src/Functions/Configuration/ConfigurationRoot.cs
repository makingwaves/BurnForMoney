namespace BurnForMoney.Functions.Configuration
{
    public class ConfigurationRoot
    {
        public StravaConfigurationSection Strava { get; set; }
        public string SqlDbConnectionString { get; set; }

        public bool IsValid()
        {
            return Strava != null &&
                Strava.ClientId > 0 && 
                !string.IsNullOrWhiteSpace(Strava.ClientSecret) && 
                !string.IsNullOrWhiteSpace(SqlDbConnectionString);
        }
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
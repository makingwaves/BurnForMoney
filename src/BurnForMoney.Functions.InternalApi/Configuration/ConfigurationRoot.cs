namespace BurnForMoney.Functions.InternalApi.Configuration
{
    public class ConfigurationRoot
    {
        public ConnectionStringsSection ConnectionStrings { get; set; }

        public bool IsValid()
        {
            return ConnectionStrings != null;
        }
    }

    public class ConnectionStringsSection
    {
        public string SqlDbConnectionString { get; set; }
    }
}
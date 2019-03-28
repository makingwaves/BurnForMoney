namespace BurnForMoney.Domain
{
    public enum Source
    {
        None,
        Strava
    }

    public static class SourceNames
    {
        public const string BurnForMoneySystem = "bfm";
        public const string AzureActiveDirectory = "aad";
        public const string Strava = "strava";
    }
}
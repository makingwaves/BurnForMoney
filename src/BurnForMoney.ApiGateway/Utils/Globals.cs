namespace BurnForMoney.ApiGateway.Utils
{
    public static class AthleteSourceNames
    {
        public const string BurnForMoneySystem = "bfm";
        public const string AzureActiveDirectory = "aad";
        public const string Strava = "strava";
    }

    public static class Globals
    {
        public const string TokenAuthScheme = "token_auth_scheme";
        public const string OidAuthScheme = "oidc_auth_scheme";
        public const string AzureScheme = "oidc_azure_scheme";

        public const string FederatedProviderTypeClaims = "federated_provider";
    }
}
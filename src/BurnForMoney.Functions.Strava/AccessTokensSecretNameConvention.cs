namespace BurnForMoney.Functions.Strava
{
    public static class AccessTokensSecretNameConvention
    {
        public const string AccessTokenSufix = "accessToken";
        public const string RefreshTokenSufix = "refreshToken";

        public static string AccessToken(string athleteId) => $"{athleteId}-{AccessTokenSufix}";
        public static string RefreshToken(string athleteId) => $"{athleteId}-{RefreshTokenSufix}";
    }

    public static class AccessTokensTag
    {
        public static string AthleteId = "athlete-id";
        public static string RefreshTokenSecretName = "refresh-token-secret-name";
    }
}
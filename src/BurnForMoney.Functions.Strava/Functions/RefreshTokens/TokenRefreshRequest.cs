namespace BurnForMoney.Functions.Strava.Functions.RefreshTokens
{
    public class TokenRefreshRequest
    {
        public int AthleteId { get; set; }
        public string EncryptedRefreshToken { get; set; }
    }
}
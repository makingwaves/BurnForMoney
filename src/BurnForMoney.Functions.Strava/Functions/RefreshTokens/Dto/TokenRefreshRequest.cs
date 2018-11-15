namespace BurnForMoney.Functions.Strava.Functions.RefreshTokens.Dto
{
    public class TokenRefreshRequest
    {
        public string AthleteId { get; set; }
        public string EncryptedRefreshToken { get; set; }
    }
}
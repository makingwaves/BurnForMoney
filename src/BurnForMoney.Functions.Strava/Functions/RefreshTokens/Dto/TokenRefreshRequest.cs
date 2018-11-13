namespace BurnForMoney.Functions.Strava.Functions.RefreshTokens.Dto
{
    public class TokenRefreshRequest
    {
        public int AthleteId { get; set; }
        public string EncryptedRefreshToken { get; set; }
    }
}
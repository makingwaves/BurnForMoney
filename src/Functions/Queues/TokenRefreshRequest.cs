namespace BurnForMoney.Functions.Queues
{
    public class TokenRefreshRequest
    {
        public int AthleteId { get; set; }
        public string EncryptedRefreshToken { get; set; }
    }
}
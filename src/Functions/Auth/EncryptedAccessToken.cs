namespace BurnForMoney.Functions.Auth
{
    public class EncryptedAccessToken
    {
        public string Token { get; }

        public EncryptedAccessToken(string token)
        {
            Token = token;
        }
    }
}
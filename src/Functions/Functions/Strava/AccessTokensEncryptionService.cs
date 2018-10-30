using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Functions.Strava
{
    public class AccessTokensEncryptionService
    {
        public static string Encrypt(string token, string encryptionKey)
        {
            var encryptedToken = Cryptography.EncryptString(token, encryptionKey);
            return encryptedToken;
        }

        public static string Decrypt(string encryptedToken, string encryptionKey)
        {
            var token = Cryptography.DecryptString(encryptedToken, encryptionKey);
            return token;
        }
    }
}
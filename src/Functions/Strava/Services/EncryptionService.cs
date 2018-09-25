using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava.Services
{
    public interface IAccessTokensEncryptionService
    {
        string DecryptAccessToken(string encryptedAccessToken);
        string EncryptAccessToken(string accessToken);
    }

    public class AccessTokensEncryptionService : IAccessTokensEncryptionService
    {
        private readonly ILogger _log;
        private readonly string _accessTokensEncryptionKey;

        public AccessTokensEncryptionService(ILogger log, string accessTokensEncryptionKey)
        {
            _log = log;
            _accessTokensEncryptionKey = accessTokensEncryptionKey;
        }

        public string DecryptAccessToken(string encryptedAccessToken)
        {
            var decryptedToken = Cryptography.DecryptString(encryptedAccessToken, _accessTokensEncryptionKey);
            _log.LogInformation("Access token has been decrypted.");

            return decryptedToken;
        }

        public string EncryptAccessToken(string accessToken)
        {
            var encryptedToken = Cryptography.EncryptString(accessToken, _accessTokensEncryptionKey);
            _log.LogInformation("Access token has been encrypted.");

            return encryptedToken;
        }
    }
}
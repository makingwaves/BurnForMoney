using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared;
using Microsoft.Azure.KeyVault;

namespace BurnForMoney.Functions.Functions.Strava
{
    public class AccessTokensEncryptionService
    {
        private const string KeyVaultSecretName = KeyVaultSecretNames.StravaTokensEncryptionKey;
        private static readonly IKeyVaultClient KeyVaultClient = KeyVaultClientFactory.Create();

        public static async Task<string> EncryptAsync(string token, string keyVaultConnectionString, string keyVaultSecretName = null)
        {
            var kvSecret = await KeyVaultClient.GetSecretAsync(keyVaultConnectionString, keyVaultSecretName ?? KeyVaultSecretName)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = kvSecret.Value;

            var encryptedToken = Cryptography.EncryptString(token, accessTokenEncryptionKey);
            return encryptedToken;
        }

        public static async Task<string> DecryptAsync(string encryptedToken, string keyVaultConnectionString, string keyVaultSecretName = null)
        {
            var kvSecret = await KeyVaultClient.GetSecretAsync(keyVaultConnectionString, keyVaultSecretName ?? KeyVaultSecretName)
                .ConfigureAwait(false);
            var accessTokenEncryptionKey = kvSecret.Value;

            var token = Cryptography.DecryptString(encryptedToken, accessTokenEncryptionKey);
            return token;
        }
    }
}
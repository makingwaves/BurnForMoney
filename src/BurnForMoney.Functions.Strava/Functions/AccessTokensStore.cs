using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace BurnForMoney.Functions.Strava.Functions
{
    public class AccessTokensStore
    {
        private static readonly IKeyVaultClient KeyVault = KeyVaultClientFactory.Create();

        public static async Task<SecretBundle> GetAccessTokenForAsync(string athleteId, string keyVaultBaseUrl)
        {
            return await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.AccessToken(athleteId));
        }

        public static async Task<SecretBundle> GetRefreshTokenForAsync(string athleteId, string keyVaultBaseUrl)
        {
            return await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));
        }

        public static async Task AddAsync(string athleteId, string accessToken, string refreshToken, DateTime accessTokenExpirationDate, string keyVaultBaseUrl)
        {
            var accessTokenSecretName = AccessTokensSecretNameConvention.AccessToken(athleteId);
            var refreshTokenSecretName = AccessTokensSecretNameConvention.RefreshToken(athleteId);

            await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                accessTokenSecretName,
                accessToken,
                secretAttributes: new SecretAttributes(enabled: true, expires: accessTokenExpirationDate),
                tags: new Dictionary<string, string>
                {
                    { AccessTokensTag.RefreshTokenSecretName, refreshTokenSecretName },
                    { AccessTokensTag.AthleteId, athleteId }
                });
            await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                refreshTokenSecretName,
                refreshToken);
        }

        public static async Task DeleteAsync(string athleteId, string keyVaultBaseUrl)
        {
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.AccessToken(athleteId));
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));
        }

        public static async Task<List<SecretItem>> GetAllSecretsAsync(string keyVaultBaseUrl)
        {
            var secretsPage = await KeyVault.GetSecretsAsync(keyVaultBaseUrl, 25);
            var secrets = secretsPage.ToList();
            var nextPageLink = secretsPage.NextPageLink;

            while (nextPageLink != null)
            {
                secretsPage = await KeyVault.GetSecretsNextAsync(nextPageLink);
                foreach (var secretItem in secretsPage)
                {
                    secrets.Add(secretItem);
                }
                nextPageLink = secretsPage.NextPageLink;
            }

            return secrets;
        }

        public static async Task DeactivateAccessTokenOfAsync(string athleteId, string keyVaultBaseUrl)
        {
            var secretIdentifier = new SecretIdentifier(keyVaultBaseUrl, AccessTokensSecretNameConvention.AccessToken(athleteId));

            await KeyVault.UpdateSecretAsync(secretIdentifier.Identifier,
                secretAttributes: new SecretAttributes(enabled: false));
        }
    }
}
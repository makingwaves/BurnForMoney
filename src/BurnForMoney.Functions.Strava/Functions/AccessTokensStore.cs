using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, SecretBundle> AccessTokensCache = new ConcurrentDictionary<string, SecretBundle>();
        private static readonly ConcurrentDictionary<string, SecretBundle> RefreshTokensCache = new ConcurrentDictionary<string, SecretBundle>();
        private static readonly IKeyVaultClient KeyVault = KeyVaultClientFactory.Create();

        public static async Task<SecretBundle> GetAccessTokenForAsync(string athleteId, string keyVaultBaseUrl)
        {
            if (AccessTokensCache.TryGetValue(athleteId, out var accessToken))
            {
                return accessToken;
            }

            accessToken = await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.AccessToken(athleteId));
            AccessTokensCache.TryAdd(athleteId, accessToken);
            return accessToken;
        }

        public static async Task<SecretBundle> GetRefreshTokenForAsync(string athleteId, string keyVaultBaseUrl)
        {
            if (RefreshTokensCache.TryGetValue(athleteId, out var refreshToken))
            {
                return refreshToken;
            }

            refreshToken = await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));
            RefreshTokensCache.TryAdd(athleteId, refreshToken);
            return refreshToken;
        }

        public static async Task AddAsync(string athleteId, string accessToken, string refreshToken, DateTime accessTokenExpirationDate, string keyVaultBaseUrl)
        {
            var accessTokenSecretName = AccessTokensSecretNameConvention.AccessToken(athleteId);
            var refreshTokenSecretName = AccessTokensSecretNameConvention.RefreshToken(athleteId);

            var accessTokenSecret = await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                accessTokenSecretName,
                accessToken,
                secretAttributes: new SecretAttributes(enabled: true, expires: accessTokenExpirationDate),
                tags: new Dictionary<string, string>
                {
                    { AccessTokensTag.RefreshTokenSecretName, refreshTokenSecretName },
                    { AccessTokensTag.AthleteId, athleteId }
                });
            var refreshTokenSecret = await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                refreshTokenSecretName,
                refreshToken);

            AccessTokensCache.TryRemove(athleteId, out var _);
            AccessTokensCache.TryAdd(athleteId, accessTokenSecret);
            RefreshTokensCache.TryRemove(athleteId, out var _);
            RefreshTokensCache.TryAdd(athleteId, refreshTokenSecret);
        }

        public static async Task DeleteAsync(string athleteId, string keyVaultBaseUrl)
        {
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.AccessToken(athleteId));
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));

            AccessTokensCache.TryRemove(athleteId, out var _);
            RefreshTokensCache.TryRemove(athleteId, out var _);
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
            AccessTokensCache.TryRemove(athleteId, out var _);
        }
    }
}
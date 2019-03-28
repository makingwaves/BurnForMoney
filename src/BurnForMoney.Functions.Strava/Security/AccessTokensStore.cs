using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Security
{
    public static class AccessTokensStore
    {
        private static readonly IMemoryCache AccessTokensCache = new MemoryCache(new MemoryDistributedCacheOptions());
        private static readonly IMemoryCache RefreshTokensCache = new MemoryCache(new MemoryDistributedCacheOptions());
        private static readonly IKeyVaultClient KeyVault = KeyVaultClientFactory.Create();

        private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions
        {
            Size = 1,
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        public static async Task<SecretBundle> GetAccessTokenForAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            if (AccessTokensCache.TryGetValue(athleteId, out SecretBundle accessToken))
            {
                return accessToken;
            }

            try
            {
                accessToken = await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                    AccessTokensSecretNameConvention.AccessToken(athleteId));
            }
            catch (KeyVaultErrorException ex)
            {
                var error = JsonConvert.DeserializeObject<KeyVaultError>(ex.Response.Content);
                if (error.Error.InnerError.Code == "SecretDisabled")
                {
                    throw new SecretDisabledException(athleteId, ex);
                }

                throw;
            }

            AccessTokensCache.Set(athleteId, accessToken, CacheEntryOptions);
            return accessToken;
        }

        public static async Task<SecretBundle> GetRefreshTokenForAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            if (RefreshTokensCache.TryGetValue(athleteId, out SecretBundle refreshToken))
            {
                return refreshToken;
            }

            refreshToken = await KeyVault.GetSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));
            RefreshTokensCache.Set(athleteId, refreshToken, CacheEntryOptions);
            return refreshToken;
        }

        public static async Task AddAsync(Guid athleteId, string accessToken, string refreshToken,
            DateTime accessTokenExpirationDate, string keyVaultBaseUrl)
        {
            var accessTokenSecretName = AccessTokensSecretNameConvention.AccessToken(athleteId);
            var refreshTokenSecretName = AccessTokensSecretNameConvention.RefreshToken(athleteId);

            var accessTokenSecret = await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                accessTokenSecretName,
                accessToken,
                secretAttributes: new SecretAttributes(enabled: true, expires: accessTokenExpirationDate),
                tags: new Dictionary<string, string>
                {
                    {AccessTokensTag.RefreshTokenSecretName, refreshTokenSecretName},
                    {AccessTokensTag.AthleteId, athleteId.ToString()}
                });
            var refreshTokenSecret = await KeyVault.SetSecretAsync(keyVaultBaseUrl,
                refreshTokenSecretName,
                refreshToken);

            AccessTokensCache.Remove(athleteId);
            AccessTokensCache.Set(athleteId, accessTokenSecret, CacheEntryOptions);
            RefreshTokensCache.Remove(athleteId);
            RefreshTokensCache.Set(athleteId, refreshTokenSecret, CacheEntryOptions);
        }

        public static async Task DeleteAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.AccessToken(athleteId));
            await KeyVault.DeleteSecretAsync(keyVaultBaseUrl,
                AccessTokensSecretNameConvention.RefreshToken(athleteId));

            AccessTokensCache.Remove(athleteId);
            RefreshTokensCache.Remove(athleteId);
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

        public static async Task DeactivateAccessTokenOfAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            var secretIdentifier =
                new SecretIdentifier(keyVaultBaseUrl, AccessTokensSecretNameConvention.AccessToken(athleteId));

            await KeyVault.UpdateSecretAsync(secretIdentifier.Identifier,
                secretAttributes: new SecretAttributes(enabled: false));
            AccessTokensCache.Remove(athleteId);
        }

        public static async Task ActivateAccessTokenOfAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            var secretIdentifier =
                new SecretIdentifier(keyVaultBaseUrl, AccessTokensSecretNameConvention.AccessToken(athleteId));

            await KeyVault.UpdateSecretAsync(secretIdentifier.Identifier,
                secretAttributes: new SecretAttributes(enabled: true));
        }
    }

    [Serializable]
    public class SecretDisabledException : KeyVaultErrorException
    {
        public SecretDisabledException(Guid athleteId, Exception inner)
            :base($"Access token for athlete with id: {athleteId} is disabled.", inner)
        {}
    }
}
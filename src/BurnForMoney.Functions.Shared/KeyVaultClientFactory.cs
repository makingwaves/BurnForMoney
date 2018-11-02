using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace BurnForMoney.Functions.Shared
{
    public class KeyVaultClientFactory
    {
        public static IKeyVaultClient Create()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }
    }
}
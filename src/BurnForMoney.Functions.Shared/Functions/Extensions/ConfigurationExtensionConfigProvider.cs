using Microsoft.Azure.WebJobs.Host.Config;

namespace BurnForMoney.Functions.Shared.Functions.Extensions
{
    public class ConfigurationExtensionConfigProvider<T> : IExtensionConfigProvider
    {
        private readonly T _configuration;

        public ConfigurationExtensionConfigProvider(T configuration)
        {
            _configuration = configuration;
        }
       
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<ConfigurationAttribute>()
                .BindToInput<T>(_ => _configuration);
        }
    }
}
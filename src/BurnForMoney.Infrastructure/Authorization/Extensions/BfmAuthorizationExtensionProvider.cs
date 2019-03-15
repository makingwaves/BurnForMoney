using Microsoft.Azure.WebJobs.Host.Config;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class BfmAuthorizationExtensionProvider : IExtensionConfigProvider
    {
        private readonly IBfmPrincipalFactory _factory;

        public BfmAuthorizationExtensionProvider(IBfmPrincipalFactory factory)
        {
            _factory = factory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            // Creates a rule that links the attribute to the binding
            var provider = new BfmAuthorizationBindingProvider(_factory);
            var rule = context.AddBindingRule<BfmAuthorizeAttribute>().Bind(provider);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class BfmAuthorizationBindingProvider : IBindingProvider
    {
        private readonly IBfmPrincipalFactory _factory;
        public BfmAuthorizationBindingProvider(IBfmPrincipalFactory factory)
        {
            _factory = factory;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            return Task.FromResult((IBinding) new BfmAuthorizationBinding(context.Parameter, _factory));
        }
    }
}
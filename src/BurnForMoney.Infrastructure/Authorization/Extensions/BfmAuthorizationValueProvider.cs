using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class BfmAuthorizationValueProvider : IValueProvider
    {
        private readonly BfmPrincipal _principal;
        
        public BfmAuthorizationValueProvider(BfmPrincipal principal)
        {
            _principal = principal;
        }

        public Task<object> GetValueAsync()
        {
            return Task.FromResult((object)_principal);
        }

        public string ToInvokeString()
        {
            return _principal?.ToString();
        }

        public Type Type { get; } = typeof(BfmAuthorizationValueProvider);
    }
}
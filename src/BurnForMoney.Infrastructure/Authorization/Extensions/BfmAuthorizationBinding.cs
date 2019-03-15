using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public interface IBfmPrincipalFactory
    {
        BfmPrincipal Create(BindingContext context);
    }


    public class BfmAuthorizationBinding : IBinding
    {
        private const string LocalAccessToken = "local_access_token";

        private readonly ParameterInfo _parameterInfo;
        private readonly IBfmPrincipalFactory _principalFactory;

        public BfmAuthorizationBinding(ParameterInfo parameterInfo, IBfmPrincipalFactory principalFactory)
        {
            _parameterInfo = parameterInfo;
            _principalFactory = principalFactory;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {             
            return Task.FromResult<IValueProvider>(
                new BfmAuthorizationValueProvider(
                    _principalFactory.Create(context)));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameterInfo.Name
            };
        }

        public bool FromAttribute { get; } = true;
    }
}
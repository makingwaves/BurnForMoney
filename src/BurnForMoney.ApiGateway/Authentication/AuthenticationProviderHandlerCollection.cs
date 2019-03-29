using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BurnForMoney.ApiGateway.Authentication
{
    public class AuthProviderHandlerComposition : IAuthProviderHandler
    {
        private readonly List<IAuthProviderHandler> _handlers;

        public AuthProviderHandlerComposition(List<IAuthProviderHandler> handlers)
        {
            _handlers = handlers;
        }

        public bool CanHandle(CookieSigningInContext context)
        {
            return _handlers.Any(h => h.CanHandle(context));
        }

        public Task<IEnumerable<Claim>> GetBfmClaimsAsync(CookieSigningInContext context, IBfmApiClient bfmApiClient)
        {
            return _handlers
                .FirstOrDefault(h => h.CanHandle(context))
                ?.GetBfmClaimsAsync(context, bfmApiClient);
        }
    }
}
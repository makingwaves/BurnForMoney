using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BurnForMoney.ApiGateway.Authentication
{
    public interface IAuthProviderHandler
    {
        bool CanHandle(CookieSigningInContext context);
        Task<IEnumerable<Claim>> GetBfmClaimsAsync(CookieSigningInContext context, IBfmApiClient bfmApiClient);
    }
}
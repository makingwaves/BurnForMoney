using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using BurnForMoney.ApiGateway.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BurnForMoney.ApiGateway.Controllers
{
    [ApiController]
    [Route("auth")]
    public class LoginController : ControllerBase
    {
        private readonly IRedirectUriValdiator _uriValdiator;

        public LoginController(IRedirectUriValdiator uriValdiator)
        {
            _uriValdiator = uriValdiator;
        }

        
        [HttpGet]
        [Route("login")]
        public IActionResult Login(
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            [FromQuery(Name = "provider")] string provider = null)
        {
            var valdiatedRedirectUri = _uriValdiator.GetDefaultIfNotValid(redirectUri);
            if (User.Identity.IsAuthenticated)
                return Redirect(valdiatedRedirectUri);

            if (provider == null || provider == "azure_ad")
            {
                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = valdiatedRedirectUri
                }, AzureADDefaults.AuthenticationScheme);
            }

            return Redirect(valdiatedRedirectUri);
        }
        

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromQuery(Name = "redirect_uri")] string redirectUri)
        {
            await HttpContext.SignOutAsync(Globals.OidAuthScheme);
            return Redirect(_uriValdiator.GetDefaultIfNotValid(redirectUri));
        }
        
        [HttpGet]
        [Route("authorize")]
        [Authorize(AuthenticationSchemes = Globals.OidAuthScheme)]
        public IActionResult Authorize()
        {
            var response = HttpContext.GetOpenIdConnectResponse();
            var request = HttpContext.GetOpenIdConnectRequest();
            
            var identity = new ClaimsIdentity(
                OpenIdConnectServerDefaults.AuthenticationScheme,
                OpenIdConnectConstants.Claims.Name,
                OpenIdConnectConstants.Claims.Role);

            identity.AddClaim(
                new Claim(OpenIdConnectConstants.Claims.Subject, User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken));

            identity.AddClaim(
                new Claim(OpenIdConnectConstants.Claims.Name, User.FindFirst(ClaimTypes.Name).Value)
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken));

            identity.AddClaims(User.Claims.Select(c => new Claim(c.Type, c.Value)
                .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken)));

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                OpenIdConnectServerDefaults.AuthenticationScheme);

            ticket.SetScopes(new[]
            {
                OpenIdConnectConstants.Scopes.OpenId

            }.Intersect(request.GetScopes()));

            ticket.SetResources("resource_server");

            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }
    }
}
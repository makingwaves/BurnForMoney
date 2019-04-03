using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using BurnForMoney.ApiGateway.Authentication;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Clients.Dto;
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
        private readonly IRedirectUriValidator _uriValidator;

        public LoginController(IRedirectUriValidator uriValidator)
        {
            _uriValidator = uriValidator;
        }

        
        [HttpGet]
        [Route("login")]
        public IActionResult Login(
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            [FromQuery(Name = "provider")] string provider = null)
        {
            var valdiatedRedirectUri = _uriValidator.GetDefaultIfNotValid(redirectUri);
            if (User.Identity.IsAuthenticated)
                return Redirect(valdiatedRedirectUri);

            if (provider == null || provider == AthleteSourceNames.AzureActiveDirectory)
            {
                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = valdiatedRedirectUri
                }, Globals.AzureScheme);
            }

            return Redirect(valdiatedRedirectUri);
        }
        

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromQuery(Name = "redirect_uri")] string redirectUri)
        {
            await HttpContext.SignOutAsync(Globals.OidAuthScheme);
            return Redirect(_uriValidator.GetDefaultIfNotValid(redirectUri));
        }
        
        [HttpGet]
        [Route("authorize")]
        [Authorize(AuthenticationSchemes = Globals.OidAuthScheme)]
        public async Task<IActionResult> Authorize([FromServices] IBfmApiClient bfmApiClient)
        {
            var response = HttpContext.GetOpenIdConnectResponse();
            var request = HttpContext.GetOpenIdConnectRequest();
            
            var identity = new ClaimsIdentity(
                OpenIdConnectServerDefaults.AuthenticationScheme,
                OpenIdConnectConstants.Claims.Name,
                OpenIdConnectConstants.Claims.Role);


            var nameId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var idSource = User.Claims.FirstOrDefault(c => c.Type == Globals.FederatedProviderTypeClaims)?.Value;

            var athlete = await bfmApiClient.GetAthleteAsync(nameId, idSource);
            if (athlete == null)
            {
                var firstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

                athlete = await bfmApiClient.CreateAthleteAndWait(Guid.Parse(nameId), new Athlete
                {
                    FirstName = firstName,
                    LastName = lastName
                }, Request.HttpContext.RequestAborted);
            }

            if (athlete == null)
                //OR redirect to error page ...
                return StatusCode((int)HttpStatusCode.InternalServerError);

            identity.AddClaim(
                new Claim(OpenIdConnectConstants.Claims.Subject, athlete.Id.ToString())
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken));

            identity.AddClaim(
                new Claim(OpenIdConnectConstants.Claims.Name, $"{athlete.FirstName} {athlete.LastName}")
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken));
            
            identity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier, athlete.Id.ToString())
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken));


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
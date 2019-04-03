using BurnForMoney.ApiGateway.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security.Claims;

namespace BurnForMoney.ApiGateway.Authentication
{
    public static class BfmAuthExt
    {
        private static readonly string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public static AuthenticationBuilder AddBfmAuth(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            return builder
                .AddCookie(Globals.OidAuthScheme, options =>
                {
                    configuration.Bind("CookieAuth", options);
                })
                .AddOpenIdConnect(Globals.AzureScheme, options =>
                {
                    options.SignInScheme = Globals.OidAuthScheme;
                    configuration.Bind("AzureAdAuth", options);
                    options.Events.OnTokenValidated = async ctx =>
                    {
                        var claims = ctx.Principal.Claims.ToList();
                        var nameId = claims.FirstOrDefault(c => c.Type == ObjectIdentifierClaimType)?.Value;
                        if (nameId == null)
                        {
                            ctx.Principal = new ClaimsPrincipal();
                            return;
                        }

                        claims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier);
                        claims.Add(new Claim(Globals.FederatedProviderTypeClaims, AthleteSourceNames.AzureActiveDirectory));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId));

                        ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Principal.Identity.AuthenticationType));
                    };
                })
                .AddOAuthValidation(Globals.TokenAuthScheme)
                .AddOpenIdConnectServer(options =>
                {
                    var oidcConfiguration = new OidcConfiguration();
                    configuration.Bind("OpenIdConnect", oidcConfiguration);

                    options.ProviderType = typeof(BfmOidcServerProvider);
                    options.AuthorizationEndpointPath = oidcConfiguration.AuthorizationEndpointPath;
                    options.UserinfoEndpointPath = oidcConfiguration.UserinfoEndpointPath;

                    options.SigningCredentials.AddEphemeralKey();
                });
        }
    }
}

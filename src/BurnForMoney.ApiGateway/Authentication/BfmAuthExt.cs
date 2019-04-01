using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BurnForMoney.ApiGateway.Authentication
{

    public static class BfmAuthExt
    {
        public static AuthenticationBuilder AddBfmAuth(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            return builder
                .AddCookie(Globals.OidAuthScheme, options =>
                {
                    configuration.Bind("CookieAuth", options);

//                    //TODO only for testing
//                    options.Cookie.SameSite = SameSiteMode.None;
//                  
                    options.Events.OnSigningIn = async context =>
                    {

                        var claimsProvider = context.HttpContext.RequestServices.GetService<IBfmApiClient>();
                        var authenticationProviderHandler = context.HttpContext.RequestServices.GetService<IAuthProviderHandler>();

                        var principalIdentity = ((ClaimsIdentity)context.Principal.Identity);

                        if (!authenticationProviderHandler.CanHandle(context))
                        {
                            context.Principal = new ClaimsPrincipal();
                            return;
                        }

                        var bfmClaims = (await authenticationProviderHandler.GetBfmClaimsAsync(context, claimsProvider))?.ToList();

                        if (bfmClaims == null)
                        {
                            context.Principal = new ClaimsPrincipal();
                            return;
                        }

                        var existingClaims = principalIdentity.Claims.Except(bfmClaims);
                        bfmClaims.AddRange(existingClaims);
                        context.Principal = new ClaimsPrincipal(new List<ClaimsIdentity> { new ClaimsIdentity(bfmClaims, principalIdentity.AuthenticationType) });
                    };
                })
                .AddAzureAD(options =>
                {
                    configuration.Bind("AzureAdAuth", options);
                    options.CookieSchemeName = Globals.OidAuthScheme;
                })
                .AddOAuthValidation(Globals.TokenAuthShecme, options =>
                    {
                        
                    })
                .AddOpenIdConnectServer(options =>
                {
                    var oidcConfiguration = new OidcConfiguration();
                    configuration.Bind("OpenIdConnect", oidcConfiguration);

                    options.ProviderType = typeof(BfmOidcServerProvider);
                    options.AuthorizationEndpointPath = oidcConfiguration.AuthorizationEndpointPath;
                    options.UserinfoEndpointPath = oidcConfiguration.UserinfoEndpointPath;

                    options.SigningCredentials.AddEphemeralKey();

                    //TODO fix this
                    options.AllowInsecureHttp = true;
                });
        }
    }
}

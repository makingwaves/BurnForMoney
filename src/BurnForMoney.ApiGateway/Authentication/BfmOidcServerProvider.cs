using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using BurnForMoney.ApiGateway.Utils;
using Microsoft.Extensions.Options;

namespace BurnForMoney.ApiGateway.Authentication
{
    public class BfmOidcServerProvider : OpenIdConnectServerProvider
    {
        private readonly IRedirectUriValidator _redirectUriValidator;
        private readonly OidcConfiguration _openIdConnectConfiguration;
        
        public BfmOidcServerProvider(IOptions<OidcConfiguration> openIdConnectConfiguration, IRedirectUriValidator redirectUriValidator)
        {
            _redirectUriValidator = redirectUriValidator;
            _openIdConnectConfiguration = openIdConnectConfiguration.Value;
        }

        public override Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            var isRequestValid = IsClientValid(context) && IsFlowValid(context) && IsRedirectUriValid(context);

            if (isRequestValid)
                context.Validate();
            else
                context.Reject("Invalid request");
            
            return Task.CompletedTask;
        }

        public override Task ApplyUserinfoResponse(ApplyUserinfoResponseContext context)
        {
            return base.ApplyUserinfoResponse(context);
        }
        
        private bool IsFlowValid(ValidateAuthorizationRequestContext context)
        {
            return context.Request.IsImplicitFlow();
        }

        private bool IsRedirectUriValid(ValidateAuthorizationRequestContext context)
        {
            return _redirectUriValidator.IsValid(context.RedirectUri);
        }

        private bool IsClientValid(ValidateAuthorizationRequestContext context)
        {
            return _openIdConnectConfiguration.AcceptableClientsIds.Any(vc => context.ClientId == vc);
        }
    }
}
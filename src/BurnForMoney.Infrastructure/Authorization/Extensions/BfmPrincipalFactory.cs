using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class BfmPrincipalFactory : IBfmPrincipalFactory
    {
        public BfmPrincipal Create(BindingContext context)
        {
            var request = context.BindingData["req"] as HttpRequest;
            if (request == null)
                return null;

            var user = request.HttpContext.User;

            return user.Identity.IsAuthenticated ? 
                BfmPrincipal.CreateAuthenticated("d") : 
                BfmPrincipal.CreateNotAuthenticated();
        }
    }
}
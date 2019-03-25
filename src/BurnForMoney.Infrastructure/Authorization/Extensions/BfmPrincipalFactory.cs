using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                return BfmPrincipal.CreateNotAuthenticated();

            var user = request.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
                return BfmPrincipal.CreateNotAuthenticated();
            
            if(!Guid.TryParse(user.Claims?.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value, out var userId))
                return BfmPrincipal.CreateNotAuthenticated();

            var firstName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var claims = user.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

            
            return BfmPrincipal.CreateAuthenticated(userId, firstName, lastName, claims);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Clients.Dto;
using BurnForMoney.ApiGateway.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BurnForMoney.ApiGateway.Authentication.AzureActiveDirectory
{
    public class AadAuthProviderHandler : IAuthProviderHandler
    {
        private static readonly string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        private static readonly string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        
        public bool CanHandle(CookieSigningInContext context)
        {
            return context.Principal.Claims.Any(c => c.Type == TenantIdClaimType);
        }

        public async Task<IEnumerable<Claim>> GetBfmClaimsAsync(CookieSigningInContext context,
            IBfmApiClient bfmApiClient)
        {
            var aadId = context.Principal.Claims.FirstOrDefault(c => c.Type == ObjectIdentifierClaimType)?.Value;
            var athlete = await bfmApiClient.GetAthleteAsync(aadId, AthleteSourceNames.AzureActiveDirectory);

            if (athlete == null)
            {
                var firstName = context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

                if (firstName == null || lastName == null)
                    return null;

                athlete = await bfmApiClient.CreateAthleteAndWait(Guid.Parse(aadId), new Athlete
                {
                    FirstName = firstName,
                    LastName = lastName
                }, context.HttpContext.RequestAborted);
            }

            if (athlete == null)
                return null;

            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, athlete.Id.ToString()),
                new Claim(ClaimTypes.Name, athlete.FirstName)
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class DevelopBfmPrincipalFactory : IBfmPrincipalFactory
    {
        private const string LocalAccessToken = "Authorization";

        public BfmPrincipal Create(BindingContext context)
        {
            var request = context.BindingData["req"] as HttpRequest;
            if (request == null || !request.Headers.ContainsKey(LocalAccessToken))
                return BfmPrincipal.CreateNotAuthenticated();

            var lcoalAuthToken = request.Headers[LocalAccessToken].First();
            var localAuthTokenSplit = lcoalAuthToken.Split('.');

            if (localAuthTokenSplit.Length != 3)
                return BfmPrincipal.CreateNotAuthenticated();

            var tokenClaimsPart = localAuthTokenSplit[1];
            tokenClaimsPart = tokenClaimsPart.PadRight(tokenClaimsPart.Length + (4 - tokenClaimsPart.Length % 4) % 4, '=');

            var rawClaims = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                Encoding.UTF8.GetString(Convert.FromBase64String(tokenClaimsPart)));

            var stringClaims = rawClaims
                .Where(kv => kv.Value?.GetType() == typeof(string))
                .ToDictionary(kv => kv.Key, kv => (string)kv.Value);

            var userId = Guid.Parse(stringClaims.FirstOrDefault(c => c.Key == "oid").Value);
            var firstName = stringClaims.FirstOrDefault(c => c.Key == "given_name").Value;
            var lastName = stringClaims.FirstOrDefault(c => c.Key== "family_name").Value;

            return BfmPrincipal.CreateAuthenticated(userId, firstName, lastName, new List<KeyValuePair<string, string>>());
        }
    }
}
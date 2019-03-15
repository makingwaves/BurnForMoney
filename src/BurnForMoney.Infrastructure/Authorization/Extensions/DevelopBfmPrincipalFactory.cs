using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public class DevelopBfmPrincipalFactory : IBfmPrincipalFactory
    {
        private const string LocalAccessToken = "local_access_token";

        public BfmPrincipal Create(BindingContext context)
        {
            var request = context.BindingData["req"] as HttpRequest;
            if (request == null && !request.Headers.ContainsKey(LocalAccessToken))
                return null;

            var lcoalAuthToken = request.Headers[LocalAccessToken];
            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                Encoding.UTF8.GetString(Convert.FromBase64String(lcoalAuthToken)));

            return BfmPrincipal.CreateAuthenticated("test");
        }
    }
}
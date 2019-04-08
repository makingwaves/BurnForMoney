using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BurnForMoney.ApiGateway.Utils.Extensions
{
    public static class ProxyHelperExt
    {
        public static Task AuthorizedProxyAsync(this Controller controller, string uri, string code, QueryString queryString = new QueryString())
        {
            return controller.ProxyAsync($"{uri}{queryString.Add("code", code)}");
        }
    }
}
using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace BurnForMoney.ApiGateway.Utils.Extensions
{
    public static class ProxyHelperExt
    {
        public static Task AuthorizedProxyAsync(this Controller controller, string uri, string code)
        {
            return controller.ProxyAsync($"{uri}?code={code}");
        }
    }
}
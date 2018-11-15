using System.Linq;
using System.Net;
using System.Net.Http;
using Polly;
using RestSharp;

namespace BurnForMoney.Functions.Shared.Web
{
    public static class RestClientExtensions
    {
        private static readonly HttpStatusCode[] HttpStatusCodesWorthRetrying = {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };

        private static readonly Polly.Retry.RetryPolicy<IRestResponse> DefaultRetryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<IRestResponse>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
            .Retry(1);

        public static IRestResponse ExecuteWithRetry(this RestClient @this, IRestRequest request)
        {
            var a = Policy
                .Handle<HttpRequestException>()
                .OrResult<IRestResponse>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .Retry(1);

            return DefaultRetryPolicy.Execute(() => @this.Execute(request));
        }
    }
}
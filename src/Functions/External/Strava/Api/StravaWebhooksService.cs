using BurnForMoney.Functions.External.Strava.Api.Model;
using Newtonsoft.Json;
using RestSharp;

namespace BurnForMoney.Functions.External.Strava.Api
{
    public class StravaWebhooksService
    {
        private const string StravaBaseUrl = "https://api.strava.com";
        private readonly RestClient _restClient;

        public StravaWebhooksService()
        {
            _restClient = new RestClient(StravaBaseUrl);
        }

        public void CreateSubscription(int clientId, string clientSecret, string callbackUrl, string callbackToken)
        {
            var request = new RestRequest("api/v3/push_subscriptions", Method.POST);
            request.AddQueryParameter("client_id", clientId.ToString());
            request.AddQueryParameter("client_secret", clientSecret);
            request.AddQueryParameter("callback_url", callbackUrl);
            request.AddQueryParameter("verify_token", callbackToken);
            var response = _restClient.Execute(request);

            response.ThrowExceptionIfNotSuccessful();
        }

        public string ViewSubscription(int clientId, string clientSecret)
        {
            var request = new RestRequest("api/v3/push_subscriptions", Method.GET);
            request.AddQueryParameter("client_id", clientId.ToString());
            request.AddQueryParameter("client_secret", clientSecret);
            var response = _restClient.Execute(request);

            response.ThrowExceptionIfNotSuccessful();
            return response.Content;
        }
    }
}
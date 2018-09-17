using BurnForMoney.Functions.Strava.Auth;
using RestSharp;

namespace BurnForMoney.Functions.Strava.Api
{
    public class StravaService
    {
        private const string StravaBaseUrl = "https://www.strava.com";
        private readonly RestClient _restClient;

        public StravaService()
        {
            _restClient = new RestClient(StravaBaseUrl);
        }

        public IRestResponse ExchangeToken(int clientId, string clientSecret,  string code)
        {
            var request = new RestRequest("/oauth/token", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var payLoad = new TokenExchangeRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code
            };
            request.AddParameter("application/json", payLoad.ToJson(), ParameterType.RequestBody);
            return _restClient.Execute(request);
        }
    }
}
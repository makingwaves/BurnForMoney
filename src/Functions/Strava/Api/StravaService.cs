using System;
using System.Net;
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
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }

            return response;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Auth;
using BurnForMoney.Functions.Strava.Model;
using Newtonsoft.Json;
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

        public TokenExchangeResponse ExchangeToken(int clientId, string clientSecret,  string code)
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
            ThrowExceptionIfNotSuccessful(response);

            return TokenExchangeResponse.FromJson(response.Content);
        }

        public IList<Activity> GetActivities(string accessToken)
        {
            var request = new RestRequest("api/v3/athlete/activities", Method.GET);
            request.AddQueryParameter("access_token", accessToken);
            var response = _restClient.Execute(request);
            ThrowExceptionIfNotSuccessful(response);

            return JsonConvert.DeserializeObject<List<Activity>>(response.Content, new JsonSettings());
        }

        private static void ThrowExceptionIfNotSuccessful(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }
        }
    }
}
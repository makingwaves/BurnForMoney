using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.External.Strava.Api.Auth;
using BurnForMoney.Functions.External.Strava.Api.Model;
using Newtonsoft.Json;
using RestSharp;

namespace BurnForMoney.Functions.External.Strava.Api
{
    public class StravaService
    {
        private const string StravaBaseUrl = "https://www.strava.com";
        private const int ActivitiesPerPage = 50;
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

        public IList<StravaActivity> GetActivities(string accessToken, DateTime? from = null, int page = 1)
        {
            var request = new RestRequest("api/v3/athlete/activities", Method.GET);
            request.AddQueryParameter("access_token", accessToken);
            if (from != null)
            {
                var fixedDate = from.Value.AddHours(-2); // Start dates in Strava are not accurate https://groups.google.com/forum/#!topic/strava-api/s1OH5mcmCo8
                request.AddQueryParameter("after", ToEpoch(fixedDate).ToString());
            }
            request.AddQueryParameter("per_page", ActivitiesPerPage.ToString());
            request.AddQueryParameter("page", page.ToString());

            var response = _restClient.Execute(request);
            ThrowExceptionIfNotSuccessful(response);

            var activities = JsonConvert.DeserializeObject<List<StravaActivity>>(response.Content, new JsonSettings());
            if (activities.Count == ActivitiesPerPage)
            {
                var nextPage = GetActivities(accessToken, from, page + 1);
                return activities.Concat(nextPage).ToList();
            }

            return activities;
        }

        public void DeauthorizeAthlete(string accessToken)
        {
            var request = new RestRequest("/oauth/deauthorize", Method.POST);
            request.AddQueryParameter("access_token", accessToken);
            var response = _restClient.Execute(request);
            ThrowExceptionIfNotSuccessful(response);

        }

        private static void ThrowExceptionIfNotSuccessful(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }
        }

        private static long ToEpoch(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
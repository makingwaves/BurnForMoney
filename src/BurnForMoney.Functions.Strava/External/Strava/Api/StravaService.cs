using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Web;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api.Auth;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using Newtonsoft.Json;
using RestSharp;

namespace BurnForMoney.Functions.Strava.External.Strava.Api
{
    public class StravaService
    {
        private const string StravaBaseUrl = "https://www.strava.com";
        private const int ActivitiesPerPage = 50;
        private readonly RestClient _restClient;
        private string[] _notFoundActivityCodes = new[] {"invalid", "not found"};

        public StravaService()
        {
            _restClient = new RestClient(StravaBaseUrl);
        }

        public TokenExchangeResult ExchangeToken(int clientId, string clientSecret, string code)
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

            var response = _restClient.ExecuteWithRetry(request);
            response.ThrowExceptionIfNotSuccessful();

            return TokenExchangeResult.FromJson(response.Content);
        }

        public TokenRefreshResult RefreshToken(int clientId, string clientSecret, string refreshToken)
        {
            var request = new RestRequest("/oauth/token", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var payLoad = new TokenRefreshRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RefreshToken = refreshToken
            };
            request.AddParameter("application/json", payLoad.ToJson(), ParameterType.RequestBody);
            var response = _restClient.ExecuteWithRetry(request);
            response.ThrowExceptionIfNotSuccessful();

            return TokenRefreshResult.FromJson(response.Content);
        }

        public StravaActivity GetActivity(string accessToken, string activityId)
        {
            var request = new RestRequest($"api/v3/activities/{activityId}");
            request.AddQueryParameter("access_token", accessToken);

            var response = _restClient.ExecuteWithRetry(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var fault = JsonConvert.DeserializeObject<Fault>(response.Content);
                if (fault.Errors.Any(error =>
                    _notFoundActivityCodes.Any(code =>
                        code.Equals(error.Code, StringComparison.InvariantCultureIgnoreCase)) &&
                    error.Resource.Equals("Activity", StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ActivityNotFoundException(activityId);
                }
            }

            response.ThrowExceptionIfNotSuccessful();

            var activity = JsonConvert.DeserializeObject<StravaActivity>(response.Content, new JsonSettings());
            return activity;
        }

        public IList<StravaActivity> GetActivities(string accessToken, DateTime? from = null, int page = 1)
        {
            var request = new RestRequest("api/v3/athlete/activities");
            request.AddQueryParameter("access_token", accessToken);
            if (from != null)
            {
                request.AddQueryParameter("after", UnitsConverter.ConvertDateTimeToEpoch(from.Value).ToString());
            }

            request.AddQueryParameter("per_page", ActivitiesPerPage.ToString());
            request.AddQueryParameter("page", page.ToString());

            var response = _restClient.ExecuteWithRetry(request);
            response.ThrowExceptionIfNotSuccessful();

            var activities = JsonConvert.DeserializeObject<List<StravaActivity>>(response.Content, new JsonSettings());
            if (activities.Count == ActivitiesPerPage)
            {
                var nextPage = GetActivities(accessToken, from, page + 1);
                return activities.Concat(nextPage).ToList();
            }

            return activities;
        }

//        public void DeauthorizeAthlete(string accessToken)
//        {
//            var request = new RestRequest("/oauth/deauthorize", Method.POST);
//            request.AddQueryParameter("access_token", accessToken);
//            var response = _restClient.ExecuteWithRetry(request);
//            response.ThrowExceptionIfNotSuccessful();
//        }
    }
//
    internal class ActivityNotFoundException : Exception 
    {
        public ActivityNotFoundException(string id)
            : base($"Activity with id: {id} does not exists.")
        {
        }

        protected ActivityNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
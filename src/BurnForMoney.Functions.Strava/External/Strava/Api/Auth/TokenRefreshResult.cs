using System;
using BurnForMoney.Functions.Strava.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Auth
{
    public class TokenRefreshResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ExpiresAt { get; set; }

        public static TokenRefreshResult FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TokenRefreshResult>(json, new JsonSettings());
        }
    }
}
using System;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Functions.Strava.External.Strava.Api.Auth
{
    public class TokenExchangeResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public Athlete Athlete { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ExpiresAt { get; set; }

        public static TokenExchangeResult FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TokenExchangeResult>(json, new JsonSettings());
        }
    }
}
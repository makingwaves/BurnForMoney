using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Strava.Api.Model;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Auth
{
    public class TokenExchangeResponse
    {
        public string AccessToken { get; set; }
        public Athlete Athlete { get; set; }

        public static TokenExchangeResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TokenExchangeResponse>(json, new JsonSettings());
        }
    }
}
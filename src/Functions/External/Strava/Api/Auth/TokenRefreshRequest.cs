using BurnForMoney.Functions.Configuration;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.External.Strava.Api.Auth
{
    public class TokenRefreshRequest
    {
        public int ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
        public string GrantType { get; } = "refresh_token";

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSettings());
        }
    }
}
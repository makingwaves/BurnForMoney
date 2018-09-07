using BurnForMoney.Functions.Configuration;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Auth
{
    public class TokenExchangeResponse
    {
        public string AccessToken { get; set; }

        public static TokenExchangeResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TokenExchangeResponse>(json, new JsonSettings());
        }
    }
}
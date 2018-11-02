using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BurnForMoney.Functions.Configuration
{
    public class JsonSettings : JsonSerializerSettings
    {
        public JsonSettings()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
        }
    }
}
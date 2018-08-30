using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BurnForMoney.Functions
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
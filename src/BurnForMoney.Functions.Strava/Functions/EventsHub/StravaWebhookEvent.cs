using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public class StravaWebhookEvent
    {
        [JsonProperty("aspect_type")]
        public AspectType AspectType { get; set; }
        [JsonProperty("event_time")]
        public long EventTime { get; set; }
        [JsonProperty("object_id")]
        public long ObjectId { get; set; }
        [JsonProperty("object_type")]
        public ObjectType ObjectType { get; set; }
        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }
        [JsonProperty("subscription_id")]
        public int SubscriptionId { get; set; }
        [JsonProperty("updates")]
        [JsonConverter(typeof(SingleOrArrayConverter<Update>))]
        public List<Update> Updates { get; set; }
    }

    public enum AspectType
    {
        Create,
        Update,
        Delete
    }

    public enum ObjectType
    {
        Activity,
        Athlete
    }

    public class Update
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("private")]
        public bool Private { get; set; }
        [JsonProperty("authorized")]
        public bool Authorized { get; set; }
    }

    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }

            return new List<T> { token.ToObject<T>() };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<T> list = (List<T>)value;
            if (list.Count == 1)
            {
                value = list[0];
            }
            serializer.Serialize(writer, value);
        }

        public override bool CanWrite => true;
    }
}
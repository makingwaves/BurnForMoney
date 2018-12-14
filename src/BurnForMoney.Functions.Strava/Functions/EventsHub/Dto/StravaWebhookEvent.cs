using System.Collections.Generic;
using BurnForMoney.Functions.Shared;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub.Dto
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
}
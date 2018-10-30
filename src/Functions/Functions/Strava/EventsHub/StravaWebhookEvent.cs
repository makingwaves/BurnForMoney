using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.Strava.EventsHub
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
}
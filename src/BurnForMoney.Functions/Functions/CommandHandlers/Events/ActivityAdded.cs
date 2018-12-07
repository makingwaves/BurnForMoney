using System;
using BurnForMoney.Infrastructure;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public class ActivityAdded : DomainEvent
    {
        [JsonProperty(PropertyName = "activityId")]
        public string ActivityId { get; set; }
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }
        [JsonProperty(PropertyName = "distaneInMeters")]
        public double DistanceInMeters { get; set; }
        [JsonProperty(PropertyName = "movingTimeInMinutes")]
        public double MovingTimeInMinutes { get; set; }

        [JsonProperty(PropertyName = "activityType")]
        public string ActivityType { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
    }
}
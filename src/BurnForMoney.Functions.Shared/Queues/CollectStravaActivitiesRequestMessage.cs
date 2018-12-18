using System;

namespace BurnForMoney.Functions.Shared.Queues
{
    public class CollectStravaActivitiesRequestMessage
    {
        public Guid AthleteId { get; set; }
        public DateTime? From { get; set; }
    }
}
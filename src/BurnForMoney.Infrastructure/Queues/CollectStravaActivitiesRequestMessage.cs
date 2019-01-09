using System;

namespace BurnForMoney.Functions.Infrastructure.Queues
{
    public class CollectStravaActivitiesRequestMessage
    {
        public Guid AthleteId { get; set; }
        public DateTime? From { get; set; }
    }
}
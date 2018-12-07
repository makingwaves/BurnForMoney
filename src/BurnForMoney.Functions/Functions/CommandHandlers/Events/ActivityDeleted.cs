using BurnForMoney.Infrastructure;

namespace BurnForMoney.Functions.Functions.CommandHandlers.Events
{
    public class ActivityDeleted : DomainEvent
    {
        public string ActivityId { get; set; }
    }
}
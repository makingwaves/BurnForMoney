namespace BurnForMoney.Infrastructure.Events
{
    public class ActivityDeleted : DomainEvent
    {
        public string ActivityId { get; set; }
    }
}
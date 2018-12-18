namespace BurnForMoney.Infrastructure
{
    public abstract class DomainEvent : IMessage
    {
        public int Version { get; set; }
    }
}
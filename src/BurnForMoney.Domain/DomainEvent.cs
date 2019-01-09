namespace BurnForMoney.Domain
{
    public abstract class DomainEvent : IMessage
    {
        public int Version { get; set; }
    }
}
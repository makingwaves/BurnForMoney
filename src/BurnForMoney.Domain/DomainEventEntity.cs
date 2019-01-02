using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Domain
{
    public class DomainEventEntity : TableEntity
    {
        public string Type { get; set; }
        public string Data { get; set; }
    }
}
using System.Collections.Generic;

namespace BurnForMoney.Functions.Infrastructure.Queues
{
    public class Notification
    {
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}
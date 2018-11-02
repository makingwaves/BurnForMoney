using System.Collections.Generic;

namespace BurnForMoney.Functions.Shared.Queues
{
    public class Notification
    {
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}
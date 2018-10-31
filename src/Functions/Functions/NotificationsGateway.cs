using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace BurnForMoney.Functions.Functions
{
    public static class NotificationsGateway
    {
        [FunctionName(FunctionsNames.NotificationsGateway)]
        public static async Task SendEmail([QueueTrigger(QueueNames.NotificationsToSend)] Notification notification, ILogger log, ExecutionContext context,
            [SendGrid(ApiKey = "SendGrid:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector)
        {
            var configuration = ApplicationConfiguration.GetSettings(context);

            var message = new SendGridMessage
            {
                From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money")
            };

            message.AddTos(notification.Recipients.Select(email => new EmailAddress(email)).ToList());   
            message.Subject = notification.Subject;
            message.HtmlContent = notification.HtmlContent;

            log.LogInformation($"Sending message to: [{string.Join(", ", notification.Recipients)}].");
            await messageCollector.AddAsync(message);
        }
    }

    public class Notification
    {
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}
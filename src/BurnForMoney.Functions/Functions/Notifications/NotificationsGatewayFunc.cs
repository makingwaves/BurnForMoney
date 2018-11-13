using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BurnForMoney.Functions.Functions.Notifications
{
    public static class NotificationsGatewayFunc
    {
        private static SendGridClient _sendGridClient;
        private static string _emailTemplate;

        [FunctionName(FunctionsNames.NotificationsGateway)]
        public static async Task SendEmail([QueueTrigger(AppQueueNames.NotificationsToSend)] Notification notification, ILogger log, ExecutionContext context)
        {
            log.LogFunctionStart(FunctionsNames.NotificationsGateway);
            var configuration = ApplicationConfiguration.GetSettings(context);

            if (_sendGridClient == null)
            {
                _sendGridClient = new SendGridClient(configuration.SendGridApiKey);
            }

            var message = new SendGridMessage
            {
                From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money")
            };

            message.AddTos(notification.Recipients.Select(email => new EmailAddress(email)).ToList());   
            message.Subject = notification.Subject;
            message.HtmlContent = ApplyTemplate(notification.HtmlContent, context);

            log.LogInformation($"Sending message to: [{string.Join(", ", notification.Recipients)}].");
            var response = await _sendGridClient.SendEmailAsync(message);

            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
            {
                log.LogInformation(FunctionsNames.NotificationsGateway, "The message has been sent.");
            }
            else
            {
                throw new EmailException(string.Join(", ", notification.Recipients), response.StatusCode.ToString());
            }
            log.LogFunctionEnd(FunctionsNames.NotificationsGateway);
        }

        private static string ApplyTemplate(string content, ExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(_emailTemplate))
            {
                var path = Path.Combine(context.FunctionAppDirectory + "\\Resources\\", "email_template.txt");
                _emailTemplate = File.ReadAllText(path);
            }

            return _emailTemplate.Replace("%%%content%%%", content);
        }
    }
}
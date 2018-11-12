using System.Net.Mail;

namespace BurnForMoney.Functions.Exceptions
{
    public class EmailException : SmtpException
    {
        public EmailException(string recipient, string statusCode)
            : base($"Failed to send email message to [{recipient}]. Status code: {statusCode}.")
        {
        }
    }
}
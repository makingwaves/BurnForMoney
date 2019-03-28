using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class EmailException : Exception
    {

        public EmailException(string recipient, string statusCode)
            : base($"Failed to send email message to [{recipient}]. Status code: {statusCode}.")
        {
        }

        protected EmailException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
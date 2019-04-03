using System;
using System.Net;
using System.Runtime.Serialization;

namespace BurnForMoney.ApiGateway.Utils.Exception
{
    [Serializable]
    public class BfmRestException : System.Exception
    {
        public readonly HttpStatusCode StatusCode;

        public BfmRestException(string errorMessage, HttpStatusCode statusCode) : base(errorMessage)
        {
            StatusCode = statusCode;
        }

        public BfmRestException(string errorMessage, HttpStatusCode statusCode, System.Exception innerException) : base(errorMessage, innerException)
        {
            StatusCode = statusCode;
        }

        protected BfmRestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

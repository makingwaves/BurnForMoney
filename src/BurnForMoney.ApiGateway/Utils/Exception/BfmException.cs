using System.Net;

namespace BurnForMoney.ApiGateway.Utils.Exception
{
    public class BfmRestException : System.Exception
    {
        public readonly HttpStatusCode StatusCode;

        public BfmRestException(string errorMessage, HttpStatusCode statusCode) : base(errorMessage)
        {
            StatusCode = statusCode;
        }

        public BfmRestException(string errorMessage, HttpStatusCode statusCode, System.Exception innserException) : base(errorMessage, innserException)
        {
            StatusCode = statusCode;
        }
    }
}

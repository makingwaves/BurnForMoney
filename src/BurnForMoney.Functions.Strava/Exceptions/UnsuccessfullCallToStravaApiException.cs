using Microsoft.Rest;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    public class UnsuccessfullCallToStravaApiException : HttpOperationException
    {
        public UnsuccessfullCallToStravaApiException(string statusCode, string content, string errorMessage)
            : base($"Strava API returned an unsuccessfull status code. Status code: {statusCode}. Content: {content}. Error message: {errorMessage ?? "null"}.")
        {
        }
    }
}
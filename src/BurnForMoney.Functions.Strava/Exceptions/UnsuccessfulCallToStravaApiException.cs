using System;
using System.Runtime.Serialization;
using Microsoft.Rest;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class UnsuccessfulCallToStravaApiException : HttpOperationException
    {
        public UnsuccessfulCallToStravaApiException(string statusCode, string content, string errorMessage)
            : base($"Strava API returned an unsuccessful status code. Status code: {statusCode}. Content: {content}. Error message: {errorMessage ?? "null"}.")
        {
        }

        protected UnsuccessfulCallToStravaApiException(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
using System;
using System.Net;
using RestSharp;

namespace BurnForMoney.Functions.Strava.External.Strava.Api
{
    public static class RestSharpExtensions
    {
        public static void ThrowExceptionIfNotSuccessful(this IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedRequestException();
            }
            
            if (!response.IsSuccessful)
            {
                throw new Exception($"Strava API returned an unsuccessfull status code. Status code: {response.StatusCode}. Content: {response.Content}. Error message: {response.ErrorMessage ?? "null"}");
            }
        }
    }
}
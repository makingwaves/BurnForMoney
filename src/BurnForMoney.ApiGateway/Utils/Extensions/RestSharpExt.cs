using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Utils.Exception;
using RestSharp;

namespace BurnForMoney.ApiGateway.Utils.Extensions
{
    public static class RestSharpExt
    {
        public static async Task<IRestResponse<T>> ThrowOnFailure<T>(this Task<IRestResponse<T>> responseTask)
        {
            var response = await responseTask;
            if (!response.IsSuccessful)
                throw new BfmRestException(response.ErrorMessage, response.StatusCode);

            return response;
        }
    }
}
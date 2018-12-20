using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes
{
    public static class CreateAthleteFunc
    {
        [FunctionName(FunctionsNames.AddAthlete)]
        public static async Task<IActionResult> CreateAthleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete")] HttpRequest req, ExecutionContext executionContext,
            ILogger log)
        {
            log.LogFunctionStart(FunctionsNames.AddAthlete);

            var requestData = await req.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CreateAthleteRequest>(requestData);
            try
            {
                ValidateRequest(model);
            }
            catch (Exception ex)
            {
                log.LogError(FunctionsNames.AddAthlete, ex.Message);
                return new BadRequestObjectResult($"Validation failed. {ex.Message}.");
            }

            throw new NotImplementedException();
        }

        private static void ValidateRequest(CreateAthleteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                throw new ArgumentNullException(nameof(request.FirstName));
            }
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                throw new ArgumentNullException(nameof(request.LastName));
            }
            if (string.IsNullOrWhiteSpace(request.ProfilePictureUrl))
            {
                throw new ArgumentNullException(nameof(request.ProfilePictureUrl));
            }
        }
    }

    public class CreateAthleteRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
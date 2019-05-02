using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.PublicApi.Calculators;
using BurnForMoney.Functions.PublicApi.Configuration;
using BurnForMoney.Functions.Shared;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.PublicApi.Functions
{
    public static class TotalNumberApi
    {
        private const string CacheKey = "api.totalnumbers";
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryDistributedCacheOptions());

        [FunctionName(FunctionNameConvention.HttpTriggerPrefix + "TotalNumbers")]
        public static async Task<IActionResult> TotalNumbers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "totalnumbers")]
            HttpRequest req,
            ILogger log, [Configuration] ConfigurationRoot configuration, 
            [Inject] ITotalNumbersCalculator totalNumbersCalculator)
        {
            if (!Cache.TryGetValue(CacheKey, out var totalNumbers))
            {
                totalNumbers = await totalNumbersCalculator.GetTotalNumbersAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    Size = 1,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                Cache.Set(CacheKey, totalNumbers, cacheEntryOptions);
            }

            return new OkObjectResult(totalNumbers);
        }

    }
}
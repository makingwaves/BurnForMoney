using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Utils;
using BurnForMoney.ApiGateway.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BurnForMoney.ApiGateway.Controllers
{
    [ApiController]
    [Route("public/api")]
    public class PublicApiController : Controller
    {
        private readonly AppConfiguration _appConfiguration;

        public PublicApiController(IOptions<AppConfiguration> appConfiguration)
        {
            _appConfiguration = appConfiguration.Value;
        }

        [HttpGet]
        [Route("totalnumbers")]
        public Task GetTotalNumbers()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.PublicApiUri}/totalnumbers", _appConfiguration.PublicApiMasterKey);
        }
    }
}
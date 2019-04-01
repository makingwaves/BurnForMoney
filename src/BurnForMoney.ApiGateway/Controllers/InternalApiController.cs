using System;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Utils;
using BurnForMoney.ApiGateway.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp.Extensions;

namespace BurnForMoney.ApiGateway.Controllers
{
    [ApiController]
    [Route("internal/api")]
    [Authorize(AuthenticationSchemes = Globals.TokenAuthShecme)]
    public class InternalApiController : Controller
    {
        private readonly AppConfiguration _appConfiguration;

        public InternalApiController(IOptions<AppConfiguration> appConfiguration)
        {
            _appConfiguration = appConfiguration.Value;
        }

        [HttpGet]
        [Route("athletes")]
        public Task GetAthletes()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/athletes", _appConfiguration.InternalApiMasterKey);
        }
        
        [HttpPost]
        [Route("athletes/finish_strava")]
        public async Task<IActionResult> FinishAddStravaAccount([FromQuery]string code, [FromServices]IBfmApiClient client)
        {
            var athleteId = User.GetBfmAthleteId();

            if (athleteId == Guid.Empty)
                return BadRequest();

            var stravaId = await client.AddStravaAccountAndWait(athleteId, code, Request.HttpContext.RequestAborted);

            if (stravaId == 0)
                return BadRequest();

            return Ok();
        }

        [HttpGet]
        [Route("ranking")]
        public Task GetRanking()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/ranking", _appConfiguration.InternalApiMasterKey);
        }

        [HttpGet]
        [Route("activities/categories")]
        public Task GetActivitiesCategories()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/activities/categories", _appConfiguration.InternalApiMasterKey);
        }

        [HttpPost]
        [Route("me/activities")]
        public Task AddActivity([FromServices]IBfmApiClient client)
        {
            var athleteId = User.GetBfmAthleteId();
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/athlete/{athleteId}/activities", _appConfiguration.InternalApiMasterKey);
        }
    }
}
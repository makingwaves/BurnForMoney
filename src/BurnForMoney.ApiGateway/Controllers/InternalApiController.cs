using System;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Utils;
using BurnForMoney.ApiGateway.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BurnForMoney.ApiGateway.Controllers
{
    [ApiController]
    [Route("internal/api")]
    [Authorize(AuthenticationSchemes = Globals.TokenAuthScheme)]
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
        [Route("athletes/{athleteId}/strava_code")]
        public async Task<IActionResult> FinishAddStravaAccount(Guid athleteId, [FromBody]string code, [FromServices]IBfmApiClient client)
        {
            var principalAthleteId = User.GetBfmAthleteId();

            if (principalAthleteId != athleteId)
                return StatusCode((int) HttpStatusCode.Forbidden);

            if (athleteId == Guid.Empty)
                return BadRequest();

            var stravaId = await client.AddStravaAccountAndWait(athleteId, code, Request.HttpContext.RequestAborted);

            if (stravaId == 0)
                return BadRequest();

            return Ok(stravaId);
        }

        [HttpPost]
        [Route("athletes/{athleteId}/activities")]
        public Task AddActivity(Guid athleteId, [FromServices]IBfmApiClient client)
        {
            var principalAthleteId = User.GetBfmAthleteId();

            if (principalAthleteId != athleteId)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }

            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/athlete/{athleteId}/activities", _appConfiguration.InternalApiMasterKey);
        }

        [HttpGet]
        [Route("athletes/{athleteId}/activities")]
        public Task GetActivities(Guid athleteId)
        {
            var principalAthleteId = User.GetBfmAthleteId();

            if (principalAthleteId != athleteId)
            {
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }

            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/athlete/{athleteId}/activities", _appConfiguration.InternalApiMasterKey);
        }

        [HttpGet]
        [Route("ranking")]
        public Task GetRanking([FromQuery] string month=null, [FromQuery]int? year=null)
        {
            var queryString = new QueryString();

            if (!string.IsNullOrEmpty(month))
                queryString = queryString.Add("month", month);

            if (year.HasValue)
                queryString = queryString.Add("year", year.ToString());

            return (this).AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/ranking", _appConfiguration.InternalApiMasterKey, queryString);
        }

        [HttpGet]
        [Route("statistics")]
        public Task GetDashboardTop()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/statistics", _appConfiguration.InternalApiMasterKey);
        }

        [HttpGet]
        [Route("activities/categories")]
        public Task GetActivitiesCategories()
        {
            return this.AuthorizedProxyAsync($"{_appConfiguration.InternalApiUri}/activities/categories", _appConfiguration.InternalApiMasterKey);
        }
    }
}
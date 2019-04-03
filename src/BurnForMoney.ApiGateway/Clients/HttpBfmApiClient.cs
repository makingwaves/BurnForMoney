using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients.Dto;
using BurnForMoney.ApiGateway.Utils;
using BurnForMoney.ApiGateway.Utils.Exception;
using BurnForMoney.ApiGateway.Utils.Extensions;
using Microsoft.Extensions.Options;
using RestSharp;

namespace BurnForMoney.ApiGateway.Clients
{
    public class HttpBfmApiClient : IBfmApiClient
    {
        private readonly AppConfiguration _appConfiguration;

        private readonly int _maxAwaitSeconds = 61;
        private readonly int _checkIntervalSeconds = 5;

        private readonly string defaultAthleteSource = AthleteSourceNames.BurnForMoneySystem;

        public HttpBfmApiClient(IOptions<AppConfiguration> appConfiguration)
        {
            _appConfiguration = appConfiguration.Value;
        }

        public async Task<Guid> CreateAthlete(Guid activeDirectoryId, Athlete athlete)
        {
            var dto = new
            {
                AadId = activeDirectoryId,
                FirstName = athlete.FirstName,
                LastName = athlete.LastName
            };

            var response = await
                new RestClient(
                        $"{_appConfiguration.InternalApiUri}/athlete/create?code={_appConfiguration.InternalApiMasterKey}")
                        .ExecutePostTaskAsync<Guid>(new RestRequest()
                        .AddJsonBody(dto))
                        .ThrowOnFailure();

            return response.Data;

        }

        public async Task<Athlete> CreateAthleteAndWait(Guid activeDirectoryId, Athlete athlete,
            CancellationToken cancellationToken)
        {
            var athleteId = await CreateAthlete(activeDirectoryId, athlete);

            if (athleteId == Guid.Empty)
                return null;

            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < _maxAwaitSeconds)
            {
                await Task.Delay(_checkIntervalSeconds * 1000, cancellationToken);
                Athlete createdAthlete = await GetAthleteAsync(athleteId.ToString(), defaultAthleteSource);

                if (createdAthlete != null)
                    return createdAthlete;
            }

            return await GetAthleteAsync(athleteId.ToString(), defaultAthleteSource);
        }

        public async Task<Athlete> GetAthleteAsync(string id, string source)
        {
            try
            {
                var response = await new RestClient(
                        $"{_appConfiguration.InternalApiUri}/athletes/{id}?source={source}&code={_appConfiguration.InternalApiMasterKey}")
                    .ExecuteGetTaskAsync<Athlete>(new RestRequest()).ThrowOnFailure();

                return response.Data;
            }
            catch (BfmRestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<int> AddStravaAccount(Guid athleteId, string code)
        {
            var response =
                await new RestClient(
                    $"{_appConfiguration.StravaApiUri}/account/{athleteId}?authCode={code}&code={_appConfiguration.StravaApiUriMasterKey}")
                    .ExecutePostTaskAsync<int>(new RestRequest())
                    .ThrowOnFailure();

            return response.Data;
        }

        public async Task<int> AddStravaAccountAndWait(Guid athleteId, string code, CancellationToken cancellationToken)
        {
            var stravaId = await AddStravaAccount(athleteId, code);
            if (stravaId == 0)
                return 0;

            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < _maxAwaitSeconds)
            {
                await Task.Delay(_checkIntervalSeconds * 1000, cancellationToken);
                var existingAthlete = await GetAthleteAsync(stravaId.ToString(), AthleteSourceNames.Strava);

                if (existingAthlete != null)
                    return stravaId;
            }

            return 0;
        }
    }
}
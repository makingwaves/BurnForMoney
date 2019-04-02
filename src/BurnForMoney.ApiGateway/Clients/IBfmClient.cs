using System;
using System.Threading;
using System.Threading.Tasks;
using BurnForMoney.ApiGateway.Clients.Dto;

namespace BurnForMoney.ApiGateway.Clients
{
    public interface IBfmApiClient
    {
        Task<Guid> CreateAthlete(Guid activeDirectoryId, Athlete athlete);
        Task<Athlete> CreateAthleteAndWait(Guid activeDirectoryId, Athlete athlete, CancellationToken cancellationToken);
        Task<Athlete> GetAthleteAsync(string id, string source);
        Task<int> AddStravaAccount(Guid athleteId, string code);
        Task<int> AddStravaAccountAndWait(Guid athleteId, string code, CancellationToken cancellationToken);
    }
}
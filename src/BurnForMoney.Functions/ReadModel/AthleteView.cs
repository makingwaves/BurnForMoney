using System.Threading.Tasks;
using BurnForMoney.Domain.Events;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Persistence;
using Dapper;

namespace BurnForMoney.Functions.ReadModel
{
    public class AthleteView : IHandles<AthleteCreated>, IHandles<AthleteDeactivated>, IHandles<AthleteActivated>
    {
        private readonly string _sqlConnectionString;

        public AthleteView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(AthleteCreated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(
                    @"INSERT INTO dbo.Athletes (Id, ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
VALUES (@Id, @ExternalId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)", new
                    {
                        message.Id,
                        message.ExternalId,
                        message.FirstName,
                        message.LastName,
                        message.ProfilePictureUrl,
                        Active = true,
                        System = message.System.ToString()
                    });
                if (affectedRows != 1)
                {
                    throw new FailedToAddAthleteException(message.Id.ToString());
                }
            }
        }

        public async Task HandleAsync(AthleteDeactivated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=0 WHERE Id=@Id", new { Id = message.AthleteId});

                if (affectedRows != 1)
                {
                    throw new FailedToDeactivateAthleteException(message.AthleteId.ToString());
                }
            }
        }

        public async Task HandleAsync(AthleteActivated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(@"UPDATE dbo.Athletes SET Active=1 WHERE Id=@Id", new { Id = message.AthleteId });

                if (affectedRows != 1)
                {
                    throw new FailedToActivateAthleteException(message.AthleteId.ToString());
                }
            }
        }
    }
}
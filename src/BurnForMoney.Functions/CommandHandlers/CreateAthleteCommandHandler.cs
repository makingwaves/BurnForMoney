using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class CreateAthleteCommandHandler : ICommandHandler<CreateAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public CreateAthleteCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(CreateAthleteCommand message)
        {
            var athlete = new Athlete(message.Id, message.ExternalId, message.ActiveDirectoryId, message.FirstName,
                message.LastName, message.ProfilePictureUrl, message.System);
            await _repository.SaveAsync(athlete, 0);
        }
    }
}
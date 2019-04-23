using System.Threading.Tasks;
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
            var athlete = new Athlete(message.Id, message.AadId, message.FirstName, message.LastName);
            await _repository.SaveAsync(athlete, 0);
        }
    }
}
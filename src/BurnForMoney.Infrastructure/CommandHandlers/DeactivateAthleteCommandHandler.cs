using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;

namespace BurnForMoney.Infrastructure.CommandHandlers
{
    public class DeactivateAthleteCommandHandler : ICommandHandler<DeactivateAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public DeactivateAthleteCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(DeactivateAthleteCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.Deactivate();
            await _repository.SaveAsync(athlete, athlete.Version);
        }
    }
}
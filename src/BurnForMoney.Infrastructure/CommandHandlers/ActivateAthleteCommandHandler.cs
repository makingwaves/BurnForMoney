using System.Threading.Tasks;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Domain.Domain;

namespace BurnForMoney.Domain.CommandHandlers
{
    public class ActivateAthleteCommandHandler : ICommandHandler<ActivateAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public ActivateAthleteCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(ActivateAthleteCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.Activate();
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}
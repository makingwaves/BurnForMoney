using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
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
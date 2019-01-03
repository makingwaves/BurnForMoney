using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
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
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}
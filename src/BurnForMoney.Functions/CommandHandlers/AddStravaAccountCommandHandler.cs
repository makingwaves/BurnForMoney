using System.Threading.Tasks;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class AddStravaAccountCommandHandler : ICommandHandler<AddStravaAccountCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public AddStravaAccountCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(AddStravaAccountCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.AddStravaAccount(message.StravaId);
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;

namespace BurnForMoney.Infrastructure.CommandHandlers
{
    public class DeleteActivityCommandHandler : ICommandHandler<DeleteActivityCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public DeleteActivityCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(DeleteActivityCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.DeleteActivity(message.Id);
            await _repository.SaveAsync(athlete, athlete.Version);
        }
    }
}
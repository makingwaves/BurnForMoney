using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
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
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}
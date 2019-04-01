using System.Threading.Tasks;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class AssignActiveDirectoryIdToAthleteCommandHandler : ICommandHandler<AssignActiveDirectoryIdToAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public AssignActiveDirectoryIdToAthleteCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(AssignActiveDirectoryIdToAthleteCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);
            
            athlete.AssignActiveDirectoryId(message.ActiveDirectoryId);
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}
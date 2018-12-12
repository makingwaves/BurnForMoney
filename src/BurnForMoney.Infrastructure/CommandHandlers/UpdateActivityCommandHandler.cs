using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;

namespace BurnForMoney.Infrastructure.CommandHandlers
{
    public class UpdateActivityCommandHandler : ICommandHandler<UpdateActivityCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public UpdateActivityCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpdateActivityCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.UpdateActivity(message.Id, message.ActivityType, message.DistanceInMeters,
                message.MovingTimeInMinutes, message.StartDate);
            await _repository.SaveAsync(athlete, athlete.Version);
        }
    }
}
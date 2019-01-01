using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Domain.Domain;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Functions.Exceptions;

namespace BurnForMoney.Functions.CommandHandlers
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
            
            Activity activityToUpdate = athlete.Activities.FirstOrDefault(a => a.Id == message.Id);
            try
            {
                athlete.UpdateActivity(message.Id, message.ActivityType, message.DistanceInMeters,
                    message.MovingTimeInMinutes, message.StartDate);
                await _repository.SaveAsync(athlete, athlete.OriginalVersion);
            } 
            catch (NoChangesDetectedException) when (activityToUpdate.Source == Source.Strava)
            {
                // do nothing, it can signal an irrelevant update (distance, time, type and date remain unchanged).
            }
        }
    }
}
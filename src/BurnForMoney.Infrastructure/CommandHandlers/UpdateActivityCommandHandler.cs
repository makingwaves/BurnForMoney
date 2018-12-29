﻿using System.Threading.Tasks;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Domain.Domain;

namespace BurnForMoney.Domain.CommandHandlers
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
            
            try
            {
                athlete.UpdateActivity(message.Id, message.ActivityType, message.DistanceInMeters,
                    message.MovingTimeInMinutes, message.StartDate);
                await _repository.SaveAsync(athlete, athlete.OriginalVersion);
            } 
            catch (NoChangesDetectedException) when (athlete.Source == Source.Strava)
            {
                // do nothing, it can signal an irrelevant update (distance, time, type and date remain unchanged).
            }
        }
    }
}
using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Events;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class AthleteActivated : AthleteEvent
    {
        public readonly Guid AthleteId;

        public AthleteActivated(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
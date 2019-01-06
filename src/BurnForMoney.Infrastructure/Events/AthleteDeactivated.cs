using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Events;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class AthleteDeactivated : AthleteEvent
    {
        public readonly Guid AthleteId;

        public AthleteDeactivated(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
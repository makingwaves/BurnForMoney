using System;
using BurnForMoney.Domain;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class AthleteActivated : DomainEvent
    {
        public readonly Guid AthleteId;

        public AthleteActivated(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
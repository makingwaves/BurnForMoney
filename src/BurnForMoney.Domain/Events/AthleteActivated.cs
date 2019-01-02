using System;

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
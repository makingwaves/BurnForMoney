using System;

namespace BurnForMoney.Domain.Events
{
    public class AthleteDeactivated : DomainEvent
    {
        public readonly Guid AthleteId;

        public AthleteDeactivated(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
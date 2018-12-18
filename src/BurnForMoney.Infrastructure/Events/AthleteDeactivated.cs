using System;

namespace BurnForMoney.Infrastructure.Events
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
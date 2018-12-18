using System;

namespace BurnForMoney.Infrastructure.Events
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
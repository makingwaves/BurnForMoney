using System;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class AthleteDeactivated : AthleteEvent
    {
        public readonly Guid AthleteId;

        public AthleteDeactivated(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
using System;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActiveDirectoryIdAssigned : AthleteEvent
    {
        public readonly Guid AthleteId;
        public readonly Guid ActiveDirectoryId;
        
        public ActiveDirectoryIdAssigned(Guid athleteId, Guid activeDirectoryId)
        {
            AthleteId = athleteId;
            ActiveDirectoryId = activeDirectoryId;
        }
    }
}
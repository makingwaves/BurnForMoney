using System;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class ActiveDirectoryIdAssigned : AthleteEvent
    {
        public readonly Guid AthleteId;
        public readonly string ActiveDirectoryId;
        
        public ActiveDirectoryIdAssigned(Guid athleteId, string activeDirectoryId)
        {
            AthleteId = athleteId;
            ActiveDirectoryId = activeDirectoryId;
        }
    }
}
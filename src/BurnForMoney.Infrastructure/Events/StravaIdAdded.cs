using System;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class StravaIdAdded : AthleteEvent
    {
        public Guid AthleteId { get; set; }
        public string StravaId { get; set; }
    }
}
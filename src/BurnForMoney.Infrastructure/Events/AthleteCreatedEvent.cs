using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    [Obsolete]
    public class AthleteCreated : AthleteEvent
    {
        public readonly Guid Id;
        public readonly string ExternalId;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly string ProfilePictureUrl;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly Source System;

        public AthleteCreated(Guid id, string externalId, string firstName, string lastName,
            string profilePictureUrl, Source system)
        {
            Id = id;
            ExternalId = externalId;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureUrl = profilePictureUrl;
            System = system;
        }
    }

    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class AthleteCreated_V2 : AthleteEvent
    {
        public readonly Guid Id;
        public readonly Guid AadId;
        public readonly string FirstName;
        public readonly string LastName;

        
        public AthleteCreated_V2(Guid id, Guid aadId, string firstName, string lastName)
        {
            Id = id;
            AadId = aadId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
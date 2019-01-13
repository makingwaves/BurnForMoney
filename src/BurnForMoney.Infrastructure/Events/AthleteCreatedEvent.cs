using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using BurnForMoney.Infrastructure.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
    public class AthleteCreated : AthleteEvent
    {
        public readonly Guid Id;
        public readonly string ExternalId;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly string ProfilePictureUrl;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly Source System;

        public AthleteCreated(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl, Source system)
        {
            Id = id;
            ExternalId = externalId;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureUrl = profilePictureUrl;
            System = system;
        }
    }
}
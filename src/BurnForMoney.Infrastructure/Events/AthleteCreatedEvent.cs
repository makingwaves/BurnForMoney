using System;
using BurnForMoney.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class AthleteCreated : DomainEvent
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
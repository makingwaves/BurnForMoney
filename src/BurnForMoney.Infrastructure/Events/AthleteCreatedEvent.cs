using System;

namespace BurnForMoney.Infrastructure.Events
{
    public class AthleteCreatedEvent : DomainEvent
    {
        public readonly Guid Id;
        public readonly string ExternalId;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly string ProfilePictureUrl;

        public AthleteCreatedEvent(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl)
        {
            Id = id;
            ExternalId = externalId;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureUrl = profilePictureUrl;
        }
    }
}
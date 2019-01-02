using System;

namespace BurnForMoney.Domain.Commands
{
    public class CreateAthleteCommand : Command
    {
        public readonly Guid Id;
        public readonly string ExternalId;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly string ProfilePictureUrl;
        public readonly Domain.Source System;

        public CreateAthleteCommand(Guid id, string externalId, string firstName, string lastName, string profilePictureUrl,
            Domain.Source system)
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
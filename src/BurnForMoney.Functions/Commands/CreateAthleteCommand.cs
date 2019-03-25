using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Commands
{
    public class CreateAthleteCommand : Command
    {
        public readonly Guid Id;
        public readonly Guid AadId;
        public readonly string FirstName;
        public readonly string LastName;
        

        public CreateAthleteCommand(Guid id, Guid aadId, string firstName, string lastName)
        {
            Id = id;
            AadId = aadId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.InternalApi.Commands
{
    public class CreateAthleteCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AadId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
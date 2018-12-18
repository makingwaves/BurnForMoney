using System;

namespace BurnForMoney.Infrastructure.Commands
{
    public class DeactivateAthleteCommand : Command
    {
        public Guid AthleteId { get; set; }
    }
}
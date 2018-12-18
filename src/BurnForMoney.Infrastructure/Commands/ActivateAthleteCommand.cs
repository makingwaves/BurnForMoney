using System;

namespace BurnForMoney.Infrastructure.Commands
{
    public class ActivateAthleteCommand : Command
    {
        public Guid AthleteId { get; set; }
    }
}
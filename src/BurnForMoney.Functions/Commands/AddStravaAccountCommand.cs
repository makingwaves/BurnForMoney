using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Commands
{
    public class AddStravaAccountCommand : Command
    {
        public Guid AthleteId { get; set; }
        public string StravaId { get; set; }
    }
}
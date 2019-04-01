using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Strava.Commands
{
    public class AddStravaAccountCommand : Command
    {
        public Guid AthleteId { get; set; }
        public string StravaId { get; set; }
    }
}
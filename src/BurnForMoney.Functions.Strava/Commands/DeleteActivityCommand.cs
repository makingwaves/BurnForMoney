using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Strava.Commands
{
    public class DeleteActivityCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
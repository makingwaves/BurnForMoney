using System;
using BurnForMoney.Domain.Commands;

namespace BurnForMoney.Functions.Strava.Commands
{
    public class DeleteActivityCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
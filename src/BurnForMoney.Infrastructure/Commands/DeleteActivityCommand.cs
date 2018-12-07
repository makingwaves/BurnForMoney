using System;

namespace BurnForMoney.Infrastructure.Commands
{
    public class DeleteActivityCommand : Command
    {
        public string Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
using System;

namespace BurnForMoney.Infrastructure.Commands
{
    public class DeleteActivityCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
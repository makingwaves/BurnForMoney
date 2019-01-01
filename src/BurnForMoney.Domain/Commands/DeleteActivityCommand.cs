using System;

namespace BurnForMoney.Domain.Commands
{
    public class DeleteActivityCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
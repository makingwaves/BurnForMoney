using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.InternalApi.Commands
{
    public class DeleteActivityCommand : Command
    {
        public Guid Id { get; set; }
        public Guid AthleteId { get; set; }
    }
}
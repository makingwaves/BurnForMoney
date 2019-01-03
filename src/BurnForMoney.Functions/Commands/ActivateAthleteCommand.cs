using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Commands
{
    public class ActivateAthleteCommand : Command
    {
        public readonly Guid AthleteId;

        public ActivateAthleteCommand(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
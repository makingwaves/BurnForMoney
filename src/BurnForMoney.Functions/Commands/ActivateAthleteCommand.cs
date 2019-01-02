using System;
using BurnForMoney.Domain.Commands;

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
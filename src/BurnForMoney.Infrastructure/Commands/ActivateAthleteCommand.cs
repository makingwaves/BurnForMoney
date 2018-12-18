using System;

namespace BurnForMoney.Infrastructure.Commands
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
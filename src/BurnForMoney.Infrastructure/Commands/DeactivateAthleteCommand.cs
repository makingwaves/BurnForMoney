using System;

namespace BurnForMoney.Infrastructure.Commands
{
    public class DeactivateAthleteCommand : Command
    {
        public readonly Guid AthleteId;

        public DeactivateAthleteCommand(Guid athleteId)
        {
            AthleteId = athleteId;
        }
    }
}
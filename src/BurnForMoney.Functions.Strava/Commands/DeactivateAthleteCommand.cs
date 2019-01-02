using System;
using BurnForMoney.Domain.Commands;

namespace BurnForMoney.Functions.Strava.Commands
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
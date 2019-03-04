using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Strava.Commands
{
    public class AssignActiveDirectoryIdToAthleteCommand : Command
    {
        public readonly Guid AthleteId;
        public readonly string ActiveDirectoryId;

        public AssignActiveDirectoryIdToAthleteCommand(Guid athleteId, string activeDirectoryId)
        {
            AthleteId = athleteId;
            ActiveDirectoryId = activeDirectoryId;
        }
    }
}
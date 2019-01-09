using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Commands
{
    public class AddActivityCommand : Command
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }

        public Guid AthleteId { get; set; }

        public DateTime StartDate { get; set; }
        public string ActivityType { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
        public Source Source { get; set; }
    }
}
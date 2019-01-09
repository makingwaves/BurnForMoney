using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class PointsLost : DomainEvent
    {
        public readonly Guid AthleteId;

        public readonly double Points;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly PointsSource Source;
        public readonly Guid CorellationId;

        public PointsLost(Guid athleteId, double points, PointsSource source, Guid corellationId)
        {
            AthleteId = athleteId;
            Points = points;
            Source = source;
            CorellationId = corellationId;
        }
    }
}
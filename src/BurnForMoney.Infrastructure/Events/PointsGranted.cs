using System;
using BurnForMoney.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//namespace lock
namespace BurnForMoney.Domain.Events
{
    public class PointsGranted : DomainEvent
    {
        public readonly Guid AthleteId;

        public readonly double Points;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly PointsSource Source;
        public readonly Guid CorellationId;

        public PointsGranted(Guid athleteId, double points, PointsSource source, Guid corellationId)
        {
            AthleteId = athleteId;
            Points = points;
            Source = source;
            CorellationId = corellationId;
        }
    }
}
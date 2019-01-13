using System;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BurnForMoney.Domain.Events
{
    [NamespaceLock(Reason = NamespaceLockAttribute.Public_Contract_Please_Do_Not_Change_Its_Namespace)]
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
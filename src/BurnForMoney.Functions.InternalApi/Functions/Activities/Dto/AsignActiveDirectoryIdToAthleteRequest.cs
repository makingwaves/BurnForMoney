using System;

namespace BurnForMoney.Functions.InternalApi.Functions.Activities.Dto
{
    public class AsignActiveDirectoryIdToAthleteRequest
    {
        public Guid AthleteId { get; set; }
        public Guid AadId { get; set; }
    }
}
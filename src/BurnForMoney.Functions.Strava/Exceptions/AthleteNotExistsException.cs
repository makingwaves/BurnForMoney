using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class AthleteNotExistsException : DataException
    {
        public AthleteNotExistsException(string athleteId, string externalAthleteId)
            : base($"Athlete with id: [{athleteId}], external id: [{externalAthleteId}] does not exists.")
        {
        }

        protected AthleteNotExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
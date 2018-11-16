using System.Data;

namespace BurnForMoney.Functions.Exceptions
{
    public class AthleteNotExistsException : DataException
    {
        public AthleteNotExistsException(string athleteId, string externalAthleteId)
            : base($"Athlete with id: [{athleteId}], external id: [{externalAthleteId}] does not exists.")
        {
        }
    }
}
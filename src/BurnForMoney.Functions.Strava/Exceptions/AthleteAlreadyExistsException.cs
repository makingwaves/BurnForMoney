using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class AthleteAlreadyExistsException : Exception
    {

        public AthleteAlreadyExistsException(string athleteId)
            : base($"{nameof(AthleteAlreadyExistsException)} Athlete with strava id: [{athleteId}] already exists..")
        {
        }

        protected AthleteAlreadyExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
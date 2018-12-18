using System;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class AthleteAlreadyExistsException : Exception
    {
        public AthleteAlreadyExistsException(string athleteId)
            : base($"{nameof(AthleteAlreadyExistsException)} Athlete with strava id: [{athleteId}] already exists..")
        {
        }

        public AthleteAlreadyExistsException()
        {
            
        }
    }
}
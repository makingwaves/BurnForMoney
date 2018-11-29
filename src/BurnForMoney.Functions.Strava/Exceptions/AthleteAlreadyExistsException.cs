using System;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class AthleteAlreadyExistsException : Exception
    {
        public AthleteAlreadyExistsException(string athleteId)
            : base($"Athlete with strava id: [{athleteId}] already exists..")
        {
        }

        public AthleteAlreadyExistsException()
        {
            
        }
    }
}
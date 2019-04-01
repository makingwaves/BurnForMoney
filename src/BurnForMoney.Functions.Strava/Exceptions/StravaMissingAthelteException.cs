using System;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class StravaMissingAthelteException : Exception
    {
        public StravaMissingAthelteException()
        {}

        public StravaMissingAthelteException(Guid athleteId)
            : base($"{nameof(StravaAccountExistsException)} Athlete account aadId: [{athleteId}] is missing or is inactive.")
        { }
    }
}
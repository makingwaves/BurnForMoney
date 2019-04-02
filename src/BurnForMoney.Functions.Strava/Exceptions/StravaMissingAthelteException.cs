using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Strava.Exceptions
{
    [Serializable]
    public class StravaMissingAthelteException : Exception
    {
        public StravaMissingAthelteException(Guid athleteId)
            : base($"{nameof(StravaAccountExistsException)} Athlete account aadId: [{athleteId}] is missing or is inactive.")
        { }

        protected StravaMissingAthelteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {}
    }
}
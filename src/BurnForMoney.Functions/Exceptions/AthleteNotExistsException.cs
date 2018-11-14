using System.Net.Mail;

namespace BurnForMoney.Functions.Exceptions
{
    public class AthleteNotExistsException : SmtpException
    {
        public AthleteNotExistsException(int athleteId, int sourceAthleteId)
            : base($"Athlete with id: [{athleteId}], external id: [{sourceAthleteId}] does not exists.")
        {
        }
    }
}
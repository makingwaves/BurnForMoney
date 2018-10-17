using System;

namespace BurnForMoney.Functions.Queues
{
    public class NewStravaAthlete
    {
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }

        public string EncryptedAccessToken { get; set; }
        public string EncryptedRefreshToken { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}